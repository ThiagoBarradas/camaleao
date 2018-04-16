using Camaleao.Core.Entities;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Camaleao.Core.Test
{
    public class TemplateTest
    {
        [Fact(DisplayName = "GIVEN Template without context and Actions use context THEN Template Is Invalid")]
        [Trait("Core", "Template")]
        public void Template_WithoutContextUseContextInActions_TempalteInvalid()
        {

            Template template = new Template(Guid.NewGuid().ToString());

            template.Request = new RequestTemplate() {
                Body = "{ }"
            };
            template.Actions.Add(new Entities.Action()
            {
                Execute = "_context{{teste}}=1"
            });


            template.IsValid().Should().BeFalse(String.Join("|", template.Notifications.Select(p => p.Message).ToArray<string>()));

        }

        [Fact(DisplayName = "GIVEN Template without context and Responses use context THEN Template Is Invalid")]
        [Trait("Core", "Template")]
        public void Template_WithoutContextUseContextInResponses_TempalteInvalid()
        {

            Template template = new Template(Guid.NewGuid().ToString());
            template.Request = new RequestTemplate()
            {
                Body = "{ }"
            };
            template.Responses = new List<ResponseTemplate>()
            {
                new ResponseTemplate()
                {
                    Body_= "{\"Transaction\":{" +
                    "               \"AmountInCents\":\"_context.{{AmountInCents}}\"," +
                    "               \"AuthorizedAmountInCents\":\"_context.{{AuthorizedAmountInCents}}\"," +
                    "               \"TransactionStatus\":\"_context.{{TransactionStatus}}\"," +
                    "               \"TransactionIdentifier\":\"_context\"," +
                    "               \"UniqueSequentialNumber\":\"_context.{{UniqueSequentialNumber}}\"," +
                    "               \"SoftDescriptor\":\"_context.{{SoftDescriptor}}\"," +
                    "               \"CreateDate\":\"_context.{{CreateDate}}\"," +
                    "               \"AuthorizedDate\":\"_context.{{AuthorizedDate}}\"," +
                    "               \"AuthorizationCode\":\"_context.{{AuthorizationCode}}\"," +
                    "               \"Cancellation\":\"_context.$$Cancellation$$\"" +
                    "                   }" +
                    "}"
                }
            };


            template.IsValid().Should().BeFalse(String.Join("|", template.Notifications.Select(p => p.Message).ToArray<string>()));
        }


        [Fact(DisplayName = "GIVEN Template without context and Responses use context THEN Template Is Invalid")]
        [Trait("Core", "Template")]
        public void Template_WithoutContextUseContextInRules_TempalteInvalid()
        {

            Template template = new Template(Guid.NewGuid().ToString());
            template.Request = new RequestTemplate()
            {
                Body = "{ }"
            };
            template.Responses = new List<ResponseTemplate>()
            {
                new ResponseTemplate()
                {
                    Body_= "{\"Transaction\":{" +
                    "               \"AmountInCents\":\"02452\"," +
                    "               \"AuthorizedAmountInCents\":\"02452\"," +
                    "               \"TransactionStatus\":\"02452\"," +
                    "               \"TransactionIdentifier\":\"02452\"," +
                    "               \"UniqueSequentialNumber\":\"02452\"," +
                    "               \"SoftDescriptor\":\"02452\"," +
                    "               \"CreateDate\":\"02452\"," +
                    "               \"AuthorizedDate\":\"02452\"," +
                    "               \"AuthorizationCode\":\"02452\"," +
                    "               \"Cancellation\":\"02452\"" +
                    "                   }" +
                    "}"
                }
            };

            template.Rules.Add(new RuleTemplate()
            {
                Expression = "_context.{{creditcard}}=='4929000000002222'"
            });


            template.IsValid().Should().BeFalse(String.Join("|", template.Notifications.Select(p => p.Message).ToArray<string>()));
        }


        [Fact(DisplayName = "GIVEN Template without context and Responses use context THEN Template Is Invalid")]
        [Trait("Core", "Template")]
        public void Template_WithoutRequest_TempalteInvalid()
        {

            Template template = new Template(Guid.NewGuid().ToString());
         
            template.Responses = new List<ResponseTemplate>()
            {
                new ResponseTemplate()
                {
                    Body_= "{\"Transaction\":{" +
                    "               \"AmountInCents\":\"02452\"," +
                    "               \"AuthorizedAmountInCents\":\"02452\"," +
                    "               \"TransactionStatus\":\"02452\"," +
                    "               \"TransactionIdentifier\":\"02452\"," +
                    "               \"UniqueSequentialNumber\":\"02452\"," +
                    "               \"SoftDescriptor\":\"02452\"," +
                    "               \"CreateDate\":\"02452\"," +
                    "               \"AuthorizedDate\":\"02452\"," +
                    "               \"AuthorizationCode\":\"02452\"," +
                    "               \"Cancellation\":\"02452\"" +
                    "                   }" +
                    "}"
                }
            };

            template.Rules.Add(new RuleTemplate()
            {
                Expression = "_context.{{creditcard}}=='4929000000002222'"
            });


            template.IsValid().Should().BeFalse(String.Join("|", template.Notifications.Select(p => p.Message).ToArray<string>()));
        }
    }
}
