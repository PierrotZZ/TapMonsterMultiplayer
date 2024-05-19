using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UsersData
{
    public Dictionary<UsersPassword, string> _UsersData = new Dictionary<UsersPassword, string>();
}
[System.Serializable]
public class UsersPassword
{
    public string Users;
    public string Password;
}
[System.Serializable]
public class ResultContainerUsers
{
    public ResultUsers[] results;
    public LinksUsers links;
}
[System.Serializable]
public class ResultUsers
{
    public string key;
    public ValueUsers value;
    public string writeLock;
    public ModifiedUsers modified;
    public CreatedUsers created;
}
[System.Serializable]
public class ValueUsers
{
    public UsersData UsersData;
    
}
[System.Serializable]
public class ModifiedUsers
{
    public string date;
}
[System.Serializable]
public class CreatedUsers
{
    public string date;
}

[System.Serializable]
public class LinksUsers
{
    public string next;
}
