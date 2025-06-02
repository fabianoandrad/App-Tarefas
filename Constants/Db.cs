using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tarefas.Constants
{
    public struct Db
    {
        public static string DB_PATH
        {
            get
            {
                return Path.Combine(FileSystem.AppDataDirectory, "tarefas.db3");
            }
        }
    }
}