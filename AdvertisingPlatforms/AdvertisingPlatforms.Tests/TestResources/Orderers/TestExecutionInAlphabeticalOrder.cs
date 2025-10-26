using Xunit.Abstractions;
using Xunit.Sdk;

namespace AdvertisingPlatforms.Tests.ResourcesTest.Orderers
{
    /// <summary>
    /// Класс для упорядочивания тестов, чтобы выполнялись в алфавитном порядке
    /// </summary>
    public class TestExecutionInAlphabeticalOrder:ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(
            IEnumerable<TTestCase> testCases) where TTestCase : ITestCase =>
            testCases.OrderBy(testCase => testCase.TestMethod.Method.Name);
    }
}
