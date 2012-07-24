﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DomainModel.Entities;
using System.ServiceModel.Web;
//описание контракта сервиса WCF
//сервис основан на подходе REST
//т.е. обращение к сервису и получение результата происходит посредством http запросов / ответов
namespace RelativesInfoService.Contracts
{
    [ServiceContract(Namespace="")]
    public interface IRelativesInfoService
    {
        //получение списка родственников персоны в формате: <детализация по родственнику, тип родственного отношения>
        //запрос должен осуществлятся методом POST
        //параметры:
        //pasportNumber - номер паспорта персоны, список родственников которой хочет получить клиент
        //передается в Url
        //filter - набор значений для фильтрации списка по любой комбинации полей таблицы Persones
        //передается в теле запроса
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRelativesList?pasportNumber={pasportNumber}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<Relative> GetRelativesList(string pasportNumber, Person filter);


        //добавление родственника персоне
        //запрос должен осуществлятся методом POST
        //параметры:
        //pasportNumber - номер паспорта персоны, которая хочет добавить родственника
        //передается в Url
        //relative - описание родственника в формате: <детализация по родственнику, тип родственного отношения>
        //передается в теле запроса
        //mode - режим выполнения операции:
        //1) auto - в этом режиме после добавления родственника персоны просматриваются другие ее родственники
        //          и, если удается определить родствнное отношение между новым родственником и существующим, 
        //          то добавляется соответсвующая запись в БД
        //2) manually - в этом режиме родственник добавится только к осуществившей запрос персоне
        //передается в Url
        //возвращаемое значение:
        //1 - операция успешно завершилась
        //0 - операцию не удалось произвести
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddRelative/{pasportNumber}/{mode}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int AddRelative(string pasportNumber, Relative relative, string mode);

        //удаление родственного отношения между двумя персонами
        //запрос должен осуществлятся методом GET
        //параметры:
        //pasportNumber - номер паспорта персоны, которая хочет удалить родственника
        //передается в Url
        //relPasportNumber - номер паспорта удаляемого родственника
        //возвращаемое значение:
        //1 - операция успешно завершилась
        //0 - операцию не удалось произвести
        [OperationContract]
        [WebGet(UriTemplate = "/DeleteRelative/{pasportNumber}/{relPasportNumber}")]
        int DeleteRelative(string pasportNumber, string relPasportNumber);

        //редактирование данных о родственнике
        //запрос должен осуществлятся методом POST
        //параметры:
        //pasportNumber - номер паспорта персоны, которая хочет редактировать родственника
        //передается в Url
        //relPasportNumber - номер паспорта редактируемого родственника
        //передается в Url
        //updatedRelative - набор обновленных данных о родственнике
        //передается в теле запроса
        //возвращаемое значение:
        //1 - операция успешно завершилась
        //0 - операцию не удалось произвести
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateRelative/{pasportNumber}/{relPasportNumber}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int UpdateRelative(string pasportNumber, string relPasportNumber, Person updatedRelative);

        //редактирование родственных отношений
        //запрос должен осуществлятся методом GET
        //параметры:
        //pasportNumber1 - номер паспорта отправившей запрос персоны
        //передается в Url
        //pasportNumber2 - номер паспорта персоны, с которой редактируется отношение
        //передается в Url
        //updatedState - обновленный тип отношения
        //передается в Url
        //mode - режим выполнения операции:
        //1) auto - в этом режиме после редактирования родственного отношения просматриваются другие родственники
        //          отправившей запрос персоны, и происходит коррекция отношений между отредактированнм
        //          родственником и остальными:
        //             - если новое родственное отношение удалось определить, то оно изменяется или добавляется,
        //               если это необходимо
        //             - если не удалось определить новое родственное отношение, то старое отношение удаляется,
        //               если оно существовало
        //2) manually - в этом режиме изменяется только запрашиваемое отношение
        //передается в Url
        //возвращаемое значение:
        //1 - операция успешно завершилась
        //0 - операцию не удалось произвести
        [OperationContract]
        [WebGet(UriTemplate = "/UpdateRelationshipState/{pasportNumber1}/{pasportNumber2}/{updatedState}/{mode}")]
        int UpdateRelationshipState(string pasportNumber1, string pasportNumber2, string updatedState, string mode);

    }
}
