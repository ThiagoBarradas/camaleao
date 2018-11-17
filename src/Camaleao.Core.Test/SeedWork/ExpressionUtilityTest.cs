using Camaleao.Core.SeedWork;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Camaleao.Core.Test.SeedWork {
    public class ExpressionUtilityTest {


        [Fact(DisplayName = "GIVEN expression with two variables THEN map return two variables")]
        [Trait("Core", "Template")]
        public void ExtractVariables_ExpressionWithTwoContext_ReturnTwoVariables() {

            var expression = "_context.{{creditcard}}=='4000000000000036' || _context.{{cpf}}=='4000000000000010'";

            var variables = ExpressionUtility.ExtractVariables(expression);

            variables.Should().NotBeNull();
            variables.Count.Should().Be(2);

        }
    }
}
