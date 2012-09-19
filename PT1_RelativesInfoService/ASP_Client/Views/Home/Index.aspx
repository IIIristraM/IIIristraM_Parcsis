<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>
 
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
 
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head" runat="server">
    <title>ASP Client</title>
</head>
<body style="height: 700px">

    <div style='text-align:right; width: 360px'>
        <label>Service output format:</label>
        <select id="SelectFormat" name="D1" style='width: 156px'>
            <option id="json">JSON</option>
            <option id="html">XML</option>
        </select><br/>
        <label>"Add" - operation mode:</label>
         <select id="SelectMode" name="D1" style='width: 156px'>
            <option id="auto">Auto</option>
            <option id="manually">Manually</option>
        </select><br/><br/>
        <label>Person passport number:</label>
        <input id="PassportTextBox" value="" style='width: 150px'/><br/><br/>
        <label>Relative passport number:</label>
        <input id="relPassportTextBox" value="" style='width: 150px'/><br/>
        <label>Relative second name:</label>
        <input id="relSecondName" value="" style='width: 150px'/><br/>
        <label>Relative first name:</label>
        <input id="relFirstName" value="" style='width: 150px'/><br/>
        <label>Relative third name:</label>
        <input id="relThirdName" value="" style='width: 150px'/><br/>
        <label>Relative sex:</label>
        <select id="SelectSex" name="D1" style='width: 156px'>
            <option id="emptySex"></option>
            <option id="male">Male</option>
            <option id="female">Female</option>
        </select><br/>
        <label>Relative date of birth:</label>
        <input id="relDateOfBirth" value="" style='width: 150px'/><br/>
        <label>Relative address:</label>
        <input id="relAddress" value="" style='width: 150px'/><br/>
        <label>Relationship state:</label>
        <select id="SelectRelationshipState" name="D1" style='width: 156px'>
            <option id="emptyRS"></option>
            <option id="son">Son</option>
            <option id="daughter">Daughter</option>
            <option id="sister">Sister</option>
            <option id="brother">Brother</option>
            <option id="father">Father</option>
            <option id="mother">Mother</option>
            <option id="grandfather">Grandfather</option>
            <option id="grandmother">Grandmother</option>
            <option id="grandson">Grandson</option>
            <option id="granddaughter">Granddaughter</option>
            <option id="aunt">Aunt</option>
            <option id="uncle">Uncle</option>
            <option id="nephew">Nephew</option>
            <option id="niece">Niece</option>
            <option id="wife">Wife</option>
            <option id="husband">Husband</option>
        </select><br/>
        <button id="GetPersonInfo" style='width: 357px'>Get Person Info</button><br/>
        <button id="AddRelative" style='width: 110px'>Add Relative</button>
        <button id="DeleteRelative" style='width: 120px'>Delete Relative</button>
        <button id="UpdateRelative" style='width: 120px'>Update Relative</button><br/>
        <button id="UpdateRelationshipState" style='width: 220px'>Update Relationship State</button>
        <button id="GetRelativesList" style='width: 134px'>Get Relatives List</button><br/><br/>
    </div>

    <div>
        <span id="spanRelativesList"></span><br/>
        <span id="spanText"></span>
    </div>
</body>
</html>
 
<script src="../../Scripts/jquery-1.4.1.js" type="text/javascript"></script>
<script type="text/javascript">

    var GetPersonInfoURL = "http://localhost:8732/Design_Time_Addresses/RESTService/GetPersonInfo";
    var GetRelativesListURL = "http://localhost:8732/Design_Time_Addresses/RESTService/GetRelativesList";
    var AddRelativeURL = "http://localhost:8732/Design_Time_Addresses/RESTService/AddRelative";
    var DeleteRelativeURL = "http://localhost:8732/Design_Time_Addresses/RESTService/DeleteRelative";
    var UpdateRelativeURL = "http://localhost:8732/Design_Time_Addresses/RESTService/UpdateRelative";
    var UpdateRelationshipStateURL = "http://localhost:8732/Design_Time_Addresses/RESTService/UpdateRelationshipState";

    var format = "json";
    var mode = "auto";
    var sex = "";
    var relationshipState = "";

    $("#html").click(function () { format = "html"; });
    $("#json").click(function () { format = "json"; });

    $("#auto").click(function () { mode = "auto"; });
    $("#manually").click(function () { mode = "manually"; });

    $("#emptySex").click(function () { sex = ""; });
    $("#male").click(function () { sex = "male"; });
    $("#female").click(function () { sex = "female"; });

    $("#emptyRS").click(function () { relationshipState = ""; });
    $("#son").click(function () { relationshipState = "son"; });
    $("#daughter").click(function () { relationshipState = "daughter"; });
    $("#sister").click(function () { relationshipState = "sister"; });
    $("#brother").click(function () { relationshipState = "brother"; });
    $("#father").click(function () { relationshipState = "father"; });
    $("#mother").click(function () { relationshipState = "mother"; });
    $("#grandfather").click(function () { relationshipState = "grandfather"; });
    $("#grandmother").click(function () { relationshipState = "grandmother"; });
    $("#grandson").click(function () { relationshipState = "grandson"; });
    $("#granddaughter").click(function () { relationshipState = "granddaughter"; });
    $("#aunt").click(function () { relationshipState = "aunt"; });
    $("#uncle").click(function () { relationshipState = "uncle"; });
    $("#nephew").click(function () { relationshipState = "nephew"; });
    $("#niece").click(function () { relationshipState = "niece"; });
    $("#wife").click(function () { relationshipState = "wife"; });
    $("#husband").click(function () { relationshipState = "husband"; });

    function makeTable(jObject) {
        var jArrayObject = jObject
        if (jObject.constructor != Array) {
            jArrayObject = new Array();
            jArrayObject[0] = jObject;
        }

        var table = document.createElement("table");
        table.setAttribute('border', '1px');
        table.setAttribute('cellpadding', '4px');
        table.setAttribute('rules', 'all');
        var tboby = document.createElement("tbody");

        var trh = document.createElement('tr');
        for (var key in jArrayObject[0]) {
            if ((jArrayObject[0][key]).constructor != Object) {
                var th = document.createElement('th');
                th.appendChild(document.createTextNode(key));
                trh.appendChild(th);
            }
            else {
                $.each(jArrayObject[0][key], function (i, v) {
                    var th = document.createElement('th');
                    th.appendChild(document.createTextNode(i));
                    trh.appendChild(th);
                });
            }
        }
        tboby.appendChild(trh);

        $.each(jArrayObject, function (i, v) {
            var tr = document.createElement('tr');
            for (var key in v) {
                if (v[key].constructor != Object) {
                    var td = document.createElement('td');
                    td.appendChild(document.createTextNode(v[key]));
                    tr.appendChild(td);
                }
                else {
                    $.each(v[key], function (i, v) {
                        var td = document.createElement('td');
                        td.appendChild(document.createTextNode(v));
                        tr.appendChild(td);
                    });
                }
            }
            tboby.appendChild(tr);
        });

        table.appendChild(tboby)

        return table;
    }

    $("#GetPersonInfo").click(function () {
     $.ajax({
            cache: false,
            type: "GET",
            async: false,
            dataType: format.toString(),
            url: GetPersonInfoURL + "?passportNumber=" + $("#PassportTextBox").attr('value'),
            success: function (person) {
                 if (format.toString() == "html") {
                    alert("SUCCESS: " + person);
                    text = person;
                    var obj = {};
                    var table = makeTable(obj);
                    $("#spanRelativesList").html("").append(table);
                } else {
                    alert(JSON.stringify("SUCCESS: " + person));
                    text = JSON.stringify(person);

                    var table = makeTable(person);
                    $("#spanRelativesList").html("").append(table);
                }
                var table2 = document.createElement("table");
                table2.setAttribute('border', '1px');
                table2.setAttribute('cellpadding', '4px');
                table2.setAttribute('rules', 'all');
                var tbody = document.createElement("tbody");
                var tr = document.createElement('tr');
                var td = document.createElement('td');
                td.appendChild(document.createTextNode(text));
                tr.appendChild(td);
                tbody.appendChild(tr);
                table2.appendChild(tbody);
                $("#spanText").html("").append(table2);
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    })

    $("#GetRelativesList").click(function () {
        var jData = {};
        if (($("#relAddress").attr('value') == "") &&
           ($("#relDateOfBirth").attr('value') == "") &&
           ($("#relFirstName").attr('value') == "") &&
           ($("#relPassportTextBox").attr('value') == "") &&
           ($("#relSecondName").attr('value') == "") &&
           ($("#relThirdName").attr('value') == "") &&
           (sex == "")) { }
        else {
            jPerson = { Address: $("#relAddress").attr('value'),
                DateOfBirth: "/Date(" + Date.parse($("#relDateOfBirth").attr('value')) + " + 0400)/",
                FirstName: $("#relFirstName").attr('value'),
                PassportNumber: $("#relPassportTextBox").attr('value'),
                PersonID: 0,
                SecondName: $("#relSecondName").attr('value'),
                Sex: sex,
                ThirdName: $("#relThirdName").attr('value')
            };
            if ($("#relDateOfBirth").attr('value') == "") jPerson.DateOfBirth = "/Date(" + Date.parse("0000-00-00") + " + 0000)/";
            jData.filter = jPerson;
        }
        alert(JSON.stringify(jData));
        var text = "";
        $.ajax({
            cache: false,
            type: "POST",
            async: false,
            url: GetRelativesListURL + "?passportNumber=" + $("#PassportTextBox").attr('value'),
            data: JSON.stringify(jData),
            contentType: "application/json",
            dataType: format.toString(),
            success: function (relatives) {
                if (format.toString() == "html") {
                    alert("SUCCESS: " + relatives);
                    text = relatives;
                    var obj = {};
                    var table = makeTable(obj);
                    $("#spanRelativesList").html("").append(table);
                } else {
                    alert(JSON.stringify("SUCCESS: " + relatives));
                    text = JSON.stringify(relatives);

                    var table = makeTable(relatives);
                    $("#spanRelativesList").html("").append(table);
                }
                var table2 = document.createElement("table");
                table2.setAttribute('border', '1px');
                table2.setAttribute('cellpadding', '4px');
                table2.setAttribute('rules', 'all');
                var tbody = document.createElement("tbody");
                var tr = document.createElement('tr');
                var td = document.createElement('td');
                td.appendChild(document.createTextNode(text));
                tr.appendChild(td);
                tbody.appendChild(tr);
                table2.appendChild(tbody);
                $("#spanText").html("").append(table2);

            },
            error: function (xhr) {
                alert("ERROR:" + xhr.responseText);
            }
        });
    });

    $("#AddRelative").click(function () {

        var jData = {};
        var relative = {};
        jPerson = { Address: $("#relAddress").attr('value'),
            DateOfBirth: "/Date(" + Date.parse($("#relDateOfBirth").attr('value')) + " + 0400)/",
            FirstName: $("#relFirstName").attr('value'),
            PassportNumber: $("#relPassportTextBox").attr('value'),
            PersonID: 0,
            SecondName: $("#relSecondName").attr('value'),
            Sex: sex,
            ThirdName: $("#relThirdName").attr('value')
        };
        if ($("#relDateOfBirth").attr('value') == "") jPerson.DateOfBirth = "/Date(" + Date.parse("0000-00-00") + " + 0000)/";
        relative.Person = jPerson;
        relative.RelationshipState = relationshipState;
        jData.relative = relative;
        alert(JSON.stringify(jData));

        $.ajax({
            cache: false,
            type: "POST",
            async: false,
            url: AddRelativeURL + "?passportNumber=" + $("#PassportTextBox").attr('value') + "&mode=" + mode,
            data: JSON.stringify(jData),
            contentType: "application/json",
            dataType: format.toString(),
            success: function (code) {
                alert(code);
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });

    $("#UpdateRelationshipState").click(function () {
        $.ajax({
            cache: false,
            type: "GET",
            async: false,
            dataType: format.toString(),
            url: UpdateRelationshipStateURL + "?passportNumber1=" + $("#PassportTextBox").attr('value') + "&passportNumber2=" + $("#relPassportTextBox").attr('value') + "&updatedState=" + relationshipState + "&mode=" + mode,
            success: function (code) {
                alert(code);
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });

    $("#DeleteRelative").click(function () {
        $.ajax({
            cache: false,
            type: "GET",
            async: false,
            dataType: format.toString(),
            url: DeleteRelativeURL + "?passportNumber=" + $("#PassportTextBox").attr('value') + "&relPassportNumber=" + $("#relPassportTextBox").attr('value'),
            success: function (code) {
                alert(code);
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });

    $("#UpdateRelative").click(function () {

        jPerson = { Address: $("#relAddress").attr('value'),
            DateOfBirth: "/Date(" + Date.parse($("#relDateOfBirth").attr('value')) + " + 0400)/",
            FirstName: $("#relFirstName").attr('value'),
            PassportNumber: $("#relPassportTextBox").attr('value'),
            PersonID: 0,
            SecondName: $("#relSecondName").attr('value'),
            Sex: sex,
            ThirdName: $("#relThirdName").attr('value')
        };
        if ($("#relDateOfBirth").attr('value') == "") jPerson.DateOfBirth = "/Date(" + Date.parse("0000-00-00") + " + 0000)/";
        var jData = {};
        jData.updatedRelative = jPerson;
        alert(JSON.stringify(jData));

        $.ajax({
            cache: false,
            type: "POST",
            async: false,
            url: UpdateRelativeURL + "?passportNumber=" + $("#PassportTextBox").attr('value') + "&relPassportNumber=" + $("#relPassportTextBox").attr('value'),
            data: JSON.stringify(jData),
            contentType: "application/json",
            dataType: format.toString(),
            success: function (code) {
                alert(code);
            },
            error: function (xhr) {
                alert(xhr.responseText);
            }
        });
    });
</script>
