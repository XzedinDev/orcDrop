using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace orcDropv2
{
    internal class Obfuscate
    {
        public static void ObfuscateSourceCode()
        {
            // Generate random characters for variable and function names
            string randomChars = Guid.NewGuid().ToString("n");

            // Obtain the assembly that holds the code to obfuscate
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Loop through all types in the assembly
            foreach (Type type in assembly.GetTypes())
            {
                // Generate a random string to replace the type name
                string randomString = Guid.NewGuid().ToString("n");

                // Rename the type
                MarshalType(type, randomString);

                // Loop through all methods in the type
                foreach (MethodInfo method in type.GetMethods())
                {
                    // Rename the method
                    MarshalMember(method, randomString, randomChars);

                    // ... Feel free to add more obfuscation techniques here ...
                }
            }
        }

        // Renames the type using reflection
        private static void MarshalType(Type type, string randomString)
        {
            RuntimeHelpers.RunClassConstructor(type.TypeHandle);
            type.TypeHandle.GetType().GetField("m_name", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(type.TypeHandle.GetType().TypeHandle, randomString.ToCharArray());
        }

        // Renames the method using reflection
        private static void MarshalMember(MethodInfo method, string randomString, string randomChars)
        {
            method.GetType().GetField("m_name", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(method.GetType().TypeHandle, randomString.ToCharArray());

            method.GetType().GetField("m_signature", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(method.GetType().TypeHandle, randomChars.ToCharArray());

            if (method.GetType().GetField("m_bIsRomInit", BindingFlags.NonPublic | BindingFlags.Instance) != null)
                method.GetType().GetField("m_bIsRomInit", BindingFlags.NonPublic | BindingFlags.Instance)
                    .SetValue(method.GetType().TypeHandle, true);
        }
    }
}
