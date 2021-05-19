using System;
using System.DirectoryServices;

namespace DelAllUsers
{
    class Program
    {

        static void Main(string[] args)
        {
            int delcount = 0;
            Boolean ignore = false;
            Boolean sys = false;
            string name = null;
            string[] ignoreList = { "guest","wdagutilityaccount" };
            string sPath = "WinNT://" + Environment.MachineName + ",computer";
            DirectoryEntry localDirectory = new DirectoryEntry(sPath);
            using (var computerEntry = localDirectory)
                foreach (DirectoryEntry childEntry in computerEntry.Children)
                    if (childEntry.SchemaClassName == "User")
                    {
                        name = childEntry.Name;
                        ignore = false;
                        sys = false;
                        for (int i = 0; i<ignoreList.Length; i++)
                        {
                            if (name.ToLower().Equals(ignoreList[i])){
                                ignore = true;
                                i = ignoreList.Length + 5;
                             }
                        }
                        if (!ignore)
                        {
                            object obGroups = childEntry.Invoke("Groups");
                            foreach (object ob in (System.Collections.IEnumerable)obGroups)
                            {
                                DirectoryEntry obGpEntry = new DirectoryEntry(ob);
                                if (obGpEntry.Name.ToLower().Equals("administrators") || obGpEntry.Name.ToLower().Equals("system managed accounts group"))
                                {
                                    sys = true;
                                }
                                obGpEntry.Close();
                            }

                            if (!sys)
                            {
                                try
                                {
                                    DirectoryEntries users = localDirectory.Children;
                                    DirectoryEntry user = users.Find(name);
                                    users.Remove(user);
                                    delcount++;
                                }
                                catch(Exception err)
                                { Console.WriteLine(err.ToString()); }
                            }
                        }
                    }
            Console.WriteLine("\t " + delcount + "Users Deleted");
        }
    }
}
