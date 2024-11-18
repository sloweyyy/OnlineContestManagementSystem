public class SmtpSettings
{
  public string Host { get; set; }
  public int Port { get; set; }
  public string Username { get; set; }
  public string Password { get; set; }
  public bool UseSSL { get; set; }
  public bool UseTLS { get; set; }
  public string FromName { get; set; }

  public SmtpSettings()
  {
    Username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? throw new Exception("SMTP_USERNAME must be set");
    FromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "Online Contest Management";
    Host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? throw new Exception("SMTP_HOST must be set");
    Port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
    Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP_PASSWORD must be set");
    UseSSL = bool.Parse(Environment.GetEnvironmentVariable("SMTP_USE_SSL") ?? "true");
    UseTLS = bool.Parse(Environment.GetEnvironmentVariable("SMTP_USE_TLS") ?? "false");
  }
}
