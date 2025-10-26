using AdvertisingPlatforms.Configurations.Builder;
using AdvertisingPlatforms.Configurations.WebApp;

namespace AdvertisingPlatforms
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ���������� ���� ����������� ��������
            builder.AddServices();

            var app = builder.Build();

            // ���������� ����������� ����������������
            app.SetupWebApplication();

            app.Run();

        }
    }
}
