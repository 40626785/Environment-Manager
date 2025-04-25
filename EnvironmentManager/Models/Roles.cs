//Represents possible application roles, represented as integers in the database.
public enum Roles 
{
    BasicUser, // Users with access to only basic pages (default, lowest access level)
    Administrator,
    EnvironmentalScientist,
    OperationsManager
}