using Camaleao.Core.Mappers;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Camaleao.Core.Test
{
    public class MapJTokenTest
    {


        [Fact(DisplayName = "GIVEN json THEN map all elements")]
        [Trait("Core", "Template")]
        public void JSon_ValidJson_MapSuccess()
        {

            StringBuilder jsonString = new StringBuilder();

            jsonString.Append("{");
            jsonString.Append("   \"Transaction\":{");
            jsonString.Append("      \"AmountInCents\":66550,");
            jsonString.Append("      \"AutomaticCapture\":false,");
            jsonString.Append("      \"Card\":{");
            jsonString.Append("         \"CardBrand\":\"Visa\",");
            jsonString.Append("         \"CardNumber\":\"4929000000001111\",");
            jsonString.Append("         \"ExpMonth\":8,");
            jsonString.Append("         \"ExpYear\":2022,");
            jsonString.Append("         \"HolderDocumentNumber\":\"36878554810\",");
            jsonString.Append("         \"HolderName\":\"MARCELO P C SCUARCIALUPI\",");
            jsonString.Append("         \"SecurityCode\":\"***\"");
            jsonString.Append("      },");
            jsonString.Append("      \"Installment\":0,");
            jsonString.Append("      \"SoftDescriptor\":\"Liv Up\",");
            jsonString.Append("      \"TransactionIdentifier\":\"371eb89c-8bb7-4216-9050-3d7bb8983c41\"");
            jsonString.Append("   }");
            jsonString.Append("}");

            JToken json = this.BuildJTokenFromString(jsonString.ToString());


            Dictionary<string, dynamic> result = json.MapperContract();

            result.Should().NotBeNull();
            result.Count.Should().Be(12);
        }


        private JToken BuildJTokenFromString(string obj)
        {
            return JsonConvert.DeserializeObject<JToken>(obj);
        }
    }
}
