using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RelativesInfoService.Contracts;
using DomainModel.Entities;
using DomainModel.Concrete;
using DomainModel.Abstract;
using System.ServiceModel.Activation;
using System.Data.Linq;

//реализация контракта сервиса
namespace RelativesInfoService.Implementations
{
    //[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class RelativesInfoService : IRelativesInfoService
    {
        public string ConStr { get; set; }
        public IPersonesRepository PersonesDB { get; set; }

        public RelativesInfoService()
        {
            //устанавливается соеденение с БД
            ConStr = @"Server =.\SQLEXPRESS; Database = PT1_DB; Trusted_Connection = yes;";
            PersonesDB = new PersonesRepository(ConStr);
        }

        public List<Relative> GetRelativesList(string pasportNumber, Person filter) 
        {
            
            List<Relative> list = new List<Relative>();
            //производим запрос к базе
            var result = from p1 in PersonesDB.Persones
                     from r in PersonesDB.Relationships
                     from p2 in PersonesDB.Persones
                     where ((p1.PassportNumber == pasportNumber) && (((p1.PersonID == r.SecondPersonID) && (p2.PersonID == r.FirstPersonID)) || ((p1.PersonID == r.FirstPersonID) && (p2.PersonID == r.SecondPersonID))))
                     select new { r, p2 };
            //фильтруем результат
            if (filter != null)
            {
                if (filter.Adress != "")
                {
                    result = result.Where(r => r.p2.Adress == filter.Adress);
                }
                if ((filter.DateOfBirth != null)&&(filter.DateOfBirth.Value.Year >= 1900))
                {
                    result = result.Where(r => (r.p2.DateOfBirth.Value.Year == filter.DateOfBirth.Value.Year) &&
                                               (r.p2.DateOfBirth.Value.Month == filter.DateOfBirth.Value.Month) &&
                                               (r.p2.DateOfBirth.Value.Day == filter.DateOfBirth.Value.Day));
                }
                if (filter.FirstName != "")
                {
                    result = result.Where(r => r.p2.FirstName == filter.FirstName);
                }
                if (filter.PassportNumber != "")
                {
                    result = result.Where(r => r.p2.PassportNumber == filter.PassportNumber);
                }
                if (filter.SecondName != "")
                {
                    result = result.Where(r => r.p2.SecondName == filter.SecondName);
                }
                if (filter.Sex != "")
                {
                    result = result.Where(r => r.p2.Sex == filter.Sex);
                }
                if (filter.ThirdName != "")
                {
                    result = result.Where(r => r.p2.ThirdName == filter.ThirdName);
                }
            }
            //создаем список
            if (result.Count() > 0)
            {
                foreach (var r in result)
                {
                    string state = "";
                    //запись в таблице Relationships имеет вид
                    // <RelationshipID, FirstPersonID, SecondPersonID, State>
                    // читается как:
                    // первая персона приходится второй тем-то
                    //пример:
                    // 1 2 4 father
                    // персона2 приходится персоне4 отцом
                    //пользователь получает ответ вида:
                    //данный человек приходится мне тем-то
                    //т.е. по логике ответа родственник должен стоять на FirstPersonID, чтобы соответствовать
                    //типу родственного отношения
                    //т.о. если в БД он находится на SecondPersonID, то отношение необходимо инвертировать
                    //пример:
                    //запись БД: Вася Пете отец
                    //запрос исходит от Васи (т.е. родственник не на FirstPersonID)
                    //ответ сервиса:
                    //Петя мне сын
                    if (r.p2.PersonID == r.r.FirstPersonID)
                    {
                        list.Add(new Relative(r.p2, r.r.State));
                    }
                    else
                    {
                        //инвертировани родственного отношения
                        #region Relationship logic
                        if ((r.r.State == "son") || (r.r.State == "daughter"))
                        {
                            if (r.p2.Sex == "male") { state = "father"; } else { state = "mother"; }
                        }
                        else if ((r.r.State == "mother") || (r.r.State == "father"))
                        {
                            if (r.p2.Sex == "male") { state = "son"; } else { state = "daughter"; }
                        }
                        else if ((r.r.State == "sister") || (r.r.State == "brother"))
                        {
                            if (r.p2.Sex == "male") { state = "brother"; } else { state = "sister"; }
                        }
                        else if ((r.r.State == "wife") || (r.r.State == "husband"))
                        {
                            if (r.p2.Sex == "male") { state = "husband"; } else { state = "wife"; }
                        }
                        else if ((r.r.State == "grandson") || (r.r.State == "granddaughter"))
                        {
                            if (r.p2.Sex == "male") { state = "grandfather"; } else { state = "grandmother"; }
                        }
                        else if ((r.r.State == "grandfather") || (r.r.State == "grandmother"))
                        {
                            if (r.p2.Sex == "male") { state = "grandson"; } else { state = "granddaughter"; }
                        }
                        else if ((r.r.State == "aunt") || (r.r.State == "uncle"))
                        {
                            if (r.p2.Sex == "male") { state = "nephew"; } else { state = "niece"; }
                        }
                        else if ((r.r.State == "nephew") || (r.r.State == "niece"))
                        {
                            if (r.p2.Sex == "male") { state = "uncle"; } else { state = "aunt"; }
                        }
                        #endregion
                        list.Add(new Relative(r.p2, state));
                    }
                }
            }
            return list;
        }

        public int AddRelative(string pasportNumber, Relative relative, string mode)
        {
            int code = 1;
            int personID = 0;
            int relativeID = 0;
            Relationship relationship;
            try
            {
                //проверяем есть ли уже родственник в таблице Persones, если нет - добавляем
                var result = from p in PersonesDB.Persones where p.PassportNumber == relative.Person.PassportNumber select p.PersonID;
                if (result.Count() == 0)
                {
                    if ((relative.Person.DateOfBirth != null)&&(relative.Person.DateOfBirth.Value.Year < 1900)) relative.Person.DateOfBirth = null;
                    ((Table<Person>)PersonesDB.Persones).InsertOnSubmit(relative.Person);
                    PersonesDB.DC.SubmitChanges();
                    relativeID = relative.Person.PersonID;
                }
                else
                {
                    relativeID = result.First();
                }
                personID = (from p in PersonesDB.Persones where p.PassportNumber == pasportNumber select p.PersonID).First();
                //запрашиваем список родственников персоны
                List<Relative> relatives = GetRelativesList(pasportNumber, null);
                //проверяем существует ли уже между персоной и родственником отношение, если нет - создаем
                if (relatives.Where(rel => rel.Person.PersonID == relativeID).Count() == 0)
                {
                    relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = personID, State = relative.RelationshipState };
                    ((Table<Relationship>)(PersonesDB.Relationships)).InsertOnSubmit(relationship);
                }
                if ((relatives.Count != 0)&&(mode == "auto"))
                {
                    foreach (var rel in relatives)
                    {
                        relationship = null;
                        string newState = "";
                        //проверяем есть ли в базе отношение между добавленным родственником и существующим, если нет - 
                        //пытаемся определить тип отношения и добавить
                        result = from r in PersonesDB.Relationships
                                 where (((r.FirstPersonID == relativeID) && (r.SecondPersonID == rel.Person.PersonID)) ||
                                       ((r.FirstPersonID == rel.Person.PersonID) && (r.SecondPersonID == relativeID)))
                                 select r.RelationshipID;
                        if ((result.Count() == 0)&&(relativeID != rel.Person.PersonID))
                        {
                            switch (relative.RelationshipState)
                            {
                                //определение типа родственного отношения, читается как:
                                //если новый родственник приходится мне тем-то(case) и у меня есть тот-то(if),
                                //то новый родственник приходится старому тем-то(newState)
                                #region Relationship logic
                                case ("son"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "brother";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                    {
                                        newState = "grandson";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "nephew";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                    {
                                        newState = "son";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("daughter"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "sister";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                    {
                                        newState = "granddaughter";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "niece";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                    {
                                        newState = "daughter";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("father"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "grandfather";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if (rel.RelationshipState == "mother")
                                    {
                                        newState = "husband";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "father";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("mother"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "grandmother";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if (rel.RelationshipState == "father")
                                    {
                                        newState = "wife";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "mother";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("sister"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "aunt";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                    {
                                        newState = "daughter";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "sister";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "grandfather") || (rel.RelationshipState == "grandmother"))
                                    {
                                        newState = "granddaughter";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "aunt") || (rel.RelationshipState == "uncle"))
                                    {
                                        newState = "niece";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "niece") || (rel.RelationshipState == "nephew"))
                                    {
                                        newState = "aunt";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("brother"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "uncle";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                    {
                                        newState = "son";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "brother";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "grandfather") || (rel.RelationshipState == "grandmother"))
                                    {
                                        newState = "grandson";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "aunt") || (rel.RelationshipState == "uncle"))
                                    {
                                        newState = "nephew";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    else if ((rel.RelationshipState == "niece") || (rel.RelationshipState == "nephew"))
                                    {
                                        newState = "uncle";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("wife"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "mother";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case ("husband"):
                                    if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                    {
                                        newState = "father";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("aunt"):
                                    if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "aunt";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("uncle"):
                                    if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "uncle";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("grandfather"):
                                    if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "grandfather";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("grandmother"):
                                     if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                    {
                                        newState = "grandmother";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("granddaughter"):
                                    if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                    {
                                        newState = "granddaughter";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("grandson"):
                                    if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                    {
                                        newState = "grandson";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("nephew"):
                                   if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                    {
                                        newState = "grandson";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                case("niece"):
                                    if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                    {
                                        newState = "granddaughter";
                                        relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                    }
                                    break;
                                #endregion
                            }
                            if (relationship != null) ((Table<Relationship>)(PersonesDB.Relationships)).InsertOnSubmit(relationship);
                        }
                    }
                }
                PersonesDB.DC.SubmitChanges();
            }
            catch (Exception e)
            {
                string message = e.Message;
                code = 0;
            }
            return code; 
        }

        public int DeleteRelative(string pasportNumber, string relPasportNumber) 
        {
            int code = 1;
            try
            {
                //ищем и удаляем запись, если запись не существует, функция вернет - 0
                var result = from r in PersonesDB.Relationships
                             from p1 in PersonesDB.Persones
                             from p2 in PersonesDB.Persones
                             where ((p1.PassportNumber == pasportNumber) &&
                                    (p2.PassportNumber == relPasportNumber) &&
                                    (((r.FirstPersonID == p2.PersonID) && (r.SecondPersonID == p1.PersonID)) ||
                                    ((r.FirstPersonID == p1.PersonID) && (r.SecondPersonID == p2.PersonID))))
                             select r;
                ((Table<Relationship>)PersonesDB.Relationships).DeleteOnSubmit(result.First());
                PersonesDB.DC.SubmitChanges();
            }
            catch (Exception e)
            {
                string message = e.Message;
                code = 0;
            }
            return code; 
        }

        public int UpdateRelative(string pasportNumber, string relPasportNumber, Person updatedRelative)
        { 
            int code = 1;
            try
            {
                //ищем родственника в БД
                var result = from r in PersonesDB.Relationships
                             from p1 in PersonesDB.Persones
                             from p2 in PersonesDB.Persones
                             where ((p1.PassportNumber == pasportNumber) &&
                                        (p2.PassportNumber == relPasportNumber) &&
                                        (((r.FirstPersonID == p2.PersonID) && (r.SecondPersonID == p1.PersonID)) ||
                                        ((r.FirstPersonID == p1.PersonID) && (r.SecondPersonID == p2.PersonID))))
                             select p2;
                //если нашли, обновляем
                if (result.Count() == 1)
                {
                    var r = result.First();
                    if (updatedRelative.Adress != "") r.Adress = updatedRelative.Adress;
                    if ((updatedRelative.DateOfBirth != null)&&(updatedRelative.DateOfBirth.Value.Year >= 1900)) r.DateOfBirth = updatedRelative.DateOfBirth;
                    if (updatedRelative.FirstName != "") r.FirstName = updatedRelative.FirstName;
                    if (updatedRelative.PassportNumber != "") r.PassportNumber = updatedRelative.PassportNumber;
                    if (updatedRelative.SecondName != "") r.SecondName = updatedRelative.SecondName;
                    if (updatedRelative.Sex != "") r.Sex = updatedRelative.Sex;
                    if (updatedRelative.ThirdName != "") r.ThirdName = updatedRelative.ThirdName;
                    PersonesDB.DC.SubmitChanges();
                }
                else
                {
                    throw new Exception("relative doesn't exist or has duplicates"); 
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                code = 0;
            }
            return code; 
        }

        public int UpdateRelationshipState(string pasportNumber1, string pasportNumber2, string updatedState, string mode)
        { 
            int code = 1;
            try
            {
                //ищем отношение в БД
                var result = from r in PersonesDB.Relationships
                             from p1 in PersonesDB.Persones
                             from p2 in PersonesDB.Persones
                             where ((p1.PassportNumber == pasportNumber1) &&
                                    (p2.PassportNumber == pasportNumber2) &&
                                    (((r.FirstPersonID == p2.PersonID) && (r.SecondPersonID == p1.PersonID)) ||
                                    ((r.FirstPersonID == p1.PersonID) && (r.SecondPersonID == p2.PersonID))))
                             select new { r, p1, p2.PersonID };
                //если нашли - обновляем
                if (result.Count() == 1)
                {
                    var r = result.First();
                    int relativeID = r.PersonID; 
                    if (r.r.SecondPersonID == r.p1.PersonID)
                    {
                        r.r.State = updatedState;
                    }
                    else
                    {
                        //если необходимо - инвертируем
                        #region Relationship logic
                        string state = "";
                        if ((updatedState == "son") || (updatedState == "daughter"))
                        {
                            if (r.p1.Sex == "male") { state = "father"; } else { state = "mother"; }
                        }
                        else if ((updatedState == "mother") || (updatedState == "father"))
                        {
                            if (r.p1.Sex == "male") { state = "son"; } else { state = "daughter"; }
                        }
                        else if ((updatedState == "sister") || (updatedState == "brother"))
                        {
                            if (r.p1.Sex == "male") { state = "brother"; } else { state = "sister"; }
                        }
                        else if ((updatedState == "wife") || (updatedState == "husband"))
                        {
                            if (r.p1.Sex == "male") { state = "husband"; } else { state = "wife"; }
                        }
                        else if ((updatedState == "grandson") || (updatedState == "granddaughter"))
                        {
                            if (r.p1.Sex == "male") { state = "grandfather"; } else { state = "grandmother"; }
                        }
                        else if ((updatedState == "grandfather") || (updatedState == "grandmother"))
                        {
                            if (r.p1.Sex == "male") { state = "grandson"; } else { state = "granddaughter"; }
                        }
                        else if ((updatedState == "aunt") || (updatedState == "uncle"))
                        {
                            if (r.p1.Sex == "male") { state = "nephew"; } else { state = "niece"; }
                        }
                        else if ((updatedState == "nephew") || (updatedState == "niece"))
                        {
                            if (r.p1.Sex == "male") { state = "uncle"; } else { state = "aunt"; }
                        }
                        #endregion
                        r.r.State = state;
                    }
                    //получаем список родственников персоны с pasportNumber1 - отправившей запрос персоны
                    List<Relative> relatives = GetRelativesList(pasportNumber1, null);
                    if ((relatives.Count != 0) && (mode == "auto"))
                    {
                        Relationship relationship;
                        foreach (var rel in relatives)
                        {
                            relationship = null;
                            string newState = "";
                            int count;
                            if (relativeID != rel.Person.PersonID)
                            {
                                //определям существует ли уже отношение между отредактированным родственником
                                //и другим родственником из списка 
                                var resultRS = from rs in PersonesDB.Relationships
                                               where (((rs.FirstPersonID == relativeID) && (rs.SecondPersonID == rel.Person.PersonID)) ||
                                                     ((rs.FirstPersonID == rel.Person.PersonID) && (rs.SecondPersonID == relativeID)))
                                               select rs;

                                count = resultRS.Count();

                                //пытаемся определить новое отношение
                                switch (updatedState)
                                {
                                    #region Relationship logic
                                    case ("son"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "brother";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                        {
                                            newState = "grandson";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "nephew";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                        {
                                            newState = "son";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("daughter"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "sister";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                        {
                                            newState = "granddaughter";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "niece";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                        {
                                            newState = "daughter";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("father"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "grandfather";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if (rel.RelationshipState == "mother")
                                        {
                                            newState = "husband";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "father";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("mother"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "grandmother";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if (rel.RelationshipState == "father")
                                        {
                                            newState = "wife";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "mother";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("sister"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "aunt";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                        {
                                            newState = "daughter";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "sister";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "grandfather") || (rel.RelationshipState == "grandmother"))
                                        {
                                            newState = "granddaughter";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "aunt") || (rel.RelationshipState == "uncle"))
                                        {
                                            newState = "niece";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "niece") || (rel.RelationshipState == "nephew"))
                                        {
                                            newState = "aunt";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("brother"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "uncle";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                        {
                                            newState = "son";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "brother";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "grandfather") || (rel.RelationshipState == "grandmother"))
                                        {
                                            newState = "grandson";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "aunt") || (rel.RelationshipState == "uncle"))
                                        {
                                            newState = "nephew";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        else if ((rel.RelationshipState == "niece") || (rel.RelationshipState == "nephew"))
                                        {
                                            newState = "uncle";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("wife"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "mother";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("husband"):
                                        if ((rel.RelationshipState == "son") || (rel.RelationshipState == "daughter"))
                                        {
                                            newState = "father";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("aunt"):
                                        if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "aunt";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("uncle"):
                                        if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "uncle";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("grandfather"):
                                        if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "grandfather";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("grandmother"):
                                        if ((rel.RelationshipState == "sister") || (rel.RelationshipState == "brother"))
                                        {
                                            newState = "grandmother";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("granddaughter"):
                                        if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                        {
                                            newState = "granddaughter";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("grandson"):
                                        if ((rel.RelationshipState == "wife") || (rel.RelationshipState == "husband"))
                                        {
                                            newState = "grandson";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("nephew"):
                                        if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                        {
                                            newState = "grandson";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    case ("niece"):
                                        if ((rel.RelationshipState == "father") || (rel.RelationshipState == "mother"))
                                        {
                                            newState = "granddaughter";
                                            relationship = new Relationship { RelationshipID = 0, FirstPersonID = relativeID, SecondPersonID = rel.Person.PersonID, State = newState };
                                        }
                                        break;
                                    #endregion
                                }

                                if (relationship != null)
                                {
                                    if (count == 0)
                                    {
                                        //если определили отношение, которого не существовало - добавляем
                                        ((Table<Relationship>)(PersonesDB.Relationships)).InsertOnSubmit(relationship);
                                    }
                                    else
                                    {
                                        //если определили отношение, которого уже существовало - заменяем, если необходимо
                                        var RS = resultRS.First();
                                        if (relationship.State != RS.State)
                                            UpdateRelationshipState(rel.Person.PassportNumber, pasportNumber2, relationship.State, "manually");
                                    }
                                }
                                else
                                {
                                    //если не определили новое отношение, но существовало старое - удаляем его
                                    if (count != 0)
                                        DeleteRelative(rel.Person.PassportNumber, pasportNumber2);
                                }
                            }
                        }
                    }
                    PersonesDB.DC.SubmitChanges();
                }
                else
                {
                  throw new Exception("relationship doesn't exist or has duplicates");
                }
            }
            catch (Exception e)
            {
                string message = e.Message;
                code = 0;
            }
            return code; 
        }

    }
}
