using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DomainModel.Entities;
using System.ServiceModel.Web;
using RelativesInfoService.Infrostructure;

namespace RelativesInfoService.Contracts
{
    /// <summary>
    ///описание контракта сервиса WCF
    ///сервис основан на подходе REST
    ///т.е. обращение к сервису и получение результата происходит посредством http запросов / ответов
    /// </summary>
    [ServiceContract(Namespace="")]
    public interface IRelativesInfoService
    {
        /// <summary>
        /// получение информации о человек по номеру паспорта
        /// запрос должен осуществлятся методом GET
        /// параметры:
        /// pasportNumber - номер паспорта персоны, информацию о которой хочет получить клиент
        /// передается в Url
        /// </summary>
        /// <param name="pasportNumber"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/GetPersonInfo?passportNumber={passportNumber}")]
        Person GetPersonInfo(string passportNumber);

        /// <summary>
        ///получение списка родственников персоны в формате: <детализация по родственнику, тип родственного отношения>
        ///запрос должен осуществлятся методом POST
        ///параметры:
        ///pasportNumber - номер паспорта персоны, список родственников которой хочет получить клиент
        ///передается в Url
        ///filter - набор значений для фильтрации списка по любой комбинации полей таблицы Persones
        ///передается в теле запроса
        /// </summary>
        /// <param name="passportNumber"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRelativesList?passportNumber={passportNumber}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<Relative> GetRelativesList(string passportNumber, Person filter);

        /// <summary>
        ///добавление родственника персоне
        ///запрос должен осуществлятся методом POST
        ///параметры:
        ///pasportNumber - номер паспорта персоны, которая хочет добавить родственника
        ///передается в Url
        ///relative - описание родственника в формате: <детализация по родственнику, тип родственного отношения>
        ///передается в теле запроса
        ///mode - режим выполнения операции:
        ///1) auto - в этом режиме после добавления родственника персоны просматриваются другие ее родственники
        ///          и, если удается определить родствнное отношение между новым родственником и существующим, 
        ///          то добавляется соответсвующая запись в БД
        ///2) manually - в этом режиме родственник добавится только к осуществившей запрос персоне
        ///передается в Url
        ///возвращаемое значение:
        ///сообщение об успехе, либо ошибке при выполнении операции
        /// </summary>
        /// <param name="passportNumber"></param>
        /// <param name="relative"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddRelative?passportNumber={passportNumber}&mode={mode}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string AddRelative(string passportNumber, Relative relative, string mode);

        /// <summary>
        ///удаление родственного отношения между двумя персонами
        ///запрос должен осуществлятся методом GET
        ///параметры:
        ///pasportNumber - номер паспорта персоны, которая хочет удалить родственника
        ///передается в Url
        ///relPasportNumber - номер паспорта удаляемого родственника
        ///возвращаемое значение:
        ///сообщение об успехе, либо ошибке при выполнении операции
        /// </summary>
        /// <param name="passportNumber"></param>
        /// <param name="relPassportNumber"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/DeleteRelative?passportNumber={passportNumber}&relPassportNumber={relPassportNumber}")]
        string DeleteRelative(string passportNumber, string relPassportNumber);

        /// <summary>
        ///редактирование данных о родственнике
        ////запрос должен осуществлятся методом POST
        ///параметры:
        ///pasportNumber - номер паспорта персоны, которая хочет редактировать родственника
        ///передается в Url
        ///relPasportNumber - номер паспорта редактируемого родственника
        ///передается в Url
        ///updatedRelative - набор обновленных данных о родственнике
        ///передается в теле запроса
        ///возвращаемое значение:
        ///сообщение об успехе, либо ошибке при выполнении операции
        /// </summary>
        /// <param name="passportNumber"></param>
        /// <param name="relPassportNumber"></param>
        /// <param name="updatedRelative"></param>
        /// <returns></returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateRelative?passportNumber={passportNumber}&relPassportNumber={relPassportNumber}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        string UpdateRelative(string passportNumber, string relPassportNumber, Person updatedRelative);

        /// <summary>
        ///редактирование родственных отношений
        ///запрос должен осуществлятся методом GET
        ///параметры:
        ///pasportNumber1 - номер паспорта отправившей запрос персоны
        ///передается в Url
        ///pasportNumber2 - номер паспорта персоны, с которой редактируется отношение
        ///передается в Url
        ///updatedState - обновленный тип отношения
        ///передается в Url
        ///mode - режим выполнения операции:
        ///1) auto - в этом режиме после редактирования родственного отношения просматриваются другие родственники
        ///          отправившей запрос персоны, и происходит коррекция отношений между отредактированнм
        ///          родственником и остальными:
        ///             - если новое родственное отношение удалось определить, то оно изменяется или добавляется,
        ///               если это необходимо
        ///             - если не удалось определить новое родственное отношение, то старое отношение удаляется,
        ///               если оно существовало
        ///2) manually - в этом режиме изменяется только запрашиваемое отношение
        ///передается в Url
        ///возвращаемое значение:
        ///сообщение об успехе, либо ошибке при выполнении операции
        /// </summary>
        /// <param name="passportNumber1"></param>
        /// <param name="passportNumber2"></param>
        /// <param name="updatedState"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateRelationshipState?passportNumber1={passportNumber1}&passportNumber2={passportNumber2}&updatedState={updatedState}&mode={mode}")]
        string UpdateRelationshipState(string passportNumber1, string passportNumber2, string updatedState, string mode);

    }
}
