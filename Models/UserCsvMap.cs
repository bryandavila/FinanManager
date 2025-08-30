using CsvHelper.Configuration;

public class UserCsvMap : ClassMap<UserCsvDto>
{
  public UserCsvMap()
  {
    Map(m => m.Name).Name("Name", "Nombre");
    Map(m => m.LastName).Name("LastName", "Apellido");
    Map(m => m.Role_ID).Name("role_ID","Role", "Role_ID"); 
    Map(m => m.UserEmail).Name("UserEmail", "Email", "Correo", "correo");
  }
}
