namespace DeliveryManagementServiceTests.UnitTest
{
    public class DeliveryUnitTest
    {
        [Fact]
        public async Task Test123()
        {
            int number1 = 5;
            int number2 = 10;

            int result = (number1 + number2);

            Assert.Equal(15, result);
        }
    }
}