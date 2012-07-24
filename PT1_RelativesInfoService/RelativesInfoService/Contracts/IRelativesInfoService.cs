using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using DomainModel.Entities;
using System.ServiceModel.Web;

namespace RelativesInfoService.Contracts
{
    [ServiceContract(Namespace="")]
    public interface IRelativesInfoService
    {

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/GetRelativesList?pasportNumber={pasportNumber}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        List<Relative> GetRelativesList(string pasportNumber, Person filter);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/AddRelative/{pasportNumber}/{mode}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int AddRelative(string pasportNumber, Relative relative, string mode);

        [OperationContract]
        [WebGet(UriTemplate = "/DeleteRelative/{pasportNumber}/{relPasportNumber}")]
        int DeleteRelative(string pasportNumber, string relPasportNumber);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/UpdateRelative/{pasportNumber}/{relPasportNumber}", BodyStyle = WebMessageBodyStyle.WrappedRequest)]
        int UpdateRelative(string pasportNumber, string relPasportNumber, Person updatedRelative);

        [OperationContract]
        [WebGet(UriTemplate = "/UpdateRelationshipState/{pasportNumber1}/{pasportNumber2}/{updatedState}/{mode}")]
        int UpdateRelationshipState(string pasportNumber1, string pasportNumber2, string updatedState, string mode);

    }
}
