using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class DatabaseManager : ManagerBase<DatabaseManager>
{
    [Header("Databases")]
    [SerializeField] List<DatabaseBase> databases = new List<DatabaseBase>();
        

    public override Task Initialize()
    {
        foreach (DatabaseBase database in databases)
        {
            if (database == null)
            {
                continue;
            }

            database.Initialize();
        }        

        return base.Initialize();
    }

    public T GetDatabase<T>() where T : DatabaseBase
    {
        foreach (DatabaseBase database in databases)
        {
            if (database is T result)
            {
                return result;
            }
        }

        Debug.Log("해당 타입의 데이터베이스 없음");

        return null;
    }

}
