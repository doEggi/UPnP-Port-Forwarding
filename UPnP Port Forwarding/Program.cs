using Open.Nat;

namespace UPnP_Port_Forwarding
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                if (args.Length > 0)
                {
                    if (args[0] == "help")
                    {
                        Console.WriteLine("Help:                                                      \n" +
                                          "                                                           \n" +
                                          "open <port> <tcp/udp>     opens port                       \n" +
                                          "close <port> <tcp/udp>    closes port                      \n" +
                                          "list                      list all open ports              \n" +
                                          "closeall                  tries to close all possible ports\n" +
                                          "                                                           \n" +
                                          "Examples:                                                  \n" +
                                          "                                                           \n" +
                                          "Open port 22334 on UDP:                                    \n" +
                                          "pf.exe open 223344 udp                                     \n" +
                                          "                                                           \n" +
                                          "Close port 4825 on TCP:                                    \n" +
                                          "pf.exe close 4825 tcp                                      \n" +
                                          "                                                           \n" +
                                          "Enjoy!                                                     \n" +
                                          "~doEggi                                                    \n" +
                                          "Github: https://github.com/doEggi/UPnP-Port-Forwarding       ");
                        return;
                    }
                    else if (args[0] == "list")
                    {
                        var devAsync = new NatDiscoverer().DiscoverDeviceAsync();
                        devAsync.Wait();
                        var dev = devAsync.Result;
                        var mapsAsync = dev.GetAllMappingsAsync();
                        mapsAsync.Wait();
                        Console.WriteLine("Every opened port:\n");
                        foreach (Mapping map in mapsAsync.Result)
                            Console.WriteLine("Public: " + map.PublicPort + "; Private: " + map.PrivatePort + "; for device " + map.PrivateIP + "-" + map.Protocol.ToString());
                        Console.WriteLine("\nDone");
                        return;
                    }
                    else if(args[0] == "closeall")
                    {
                        var devAsync = new NatDiscoverer().DiscoverDeviceAsync();
                        devAsync.Wait();
                        var dev = devAsync.Result;
                        var mapsAsync = dev.GetAllMappingsAsync();
                        mapsAsync.Wait();
                        Console.WriteLine("Start:\n");
                        foreach (Mapping map in mapsAsync.Result)
                        {
                            Task t = dev.DeletePortMapAsync(map);
                            t.Wait();
                            if (t.Exception != null)
                                Console.WriteLine(t.Exception.GetType().Name + ": " + t.Exception.Message + "  (Port " + map.PublicPort + ")");
                            else
                                Console.WriteLine("Port " + map.PublicPort + " closed!");
                        }
                        Console.WriteLine("\nFinished!");
                        return;
                    }
                    else if(args.Length == 3)
                    {
                        if(args[0] == "open")
                        {
                            int port = int.Parse(args[1]);
                            if (port <= 1024 || port > 65535)
                                throw new ArgumentException("The port has to be larger than 1024 and lower than 65536!");
                            var devAsync = new NatDiscoverer().DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource());
                            devAsync.Wait();
                            var dev = devAsync.Result;
                            int a = 0;
                            if (args[2].ToLower() == "tcp")
                                a = 1;
                            else if (args[2].ToLower() == "udp")
                                a = 2;
                            if (a == 0)
                                throw new ArgumentException("The third argument has to be \"tcp\" or \"udp\"!");
                            Mapping map = new Mapping((Protocol)(a - 1), port, port);
                            dev.CreatePortMapAsync(map).Wait();
                            Console.WriteLine("Port opened!");
                            return;
                        }
                        else if(args[0] == "close")
                        {
                            int port = int.Parse(args[1]);
                            if (port <= 1024 || port > 65535)
                                throw new ArgumentException("The port has to be larger than 1024 and lower than 65536!");
                            var devAsync = new NatDiscoverer().DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource());
                            devAsync.Wait();
                            var dev = devAsync.Result;
                            int a = 0;
                            if (args[2].ToLower() == "tcp")
                                a = 1;
                            else if (args[2].ToLower() == "udp")
                                a = 2;
                            if (a == 0)
                                throw new ArgumentException("The third argument has to be \"tcp\" or \"udp\"!");
                            Mapping map = new Mapping((Protocol)(a - 1), port, port);
                            dev.DeletePortMapAsync(map);
                            Console.WriteLine("Port closed!");
                            return;
                        }
                        else
                            throw new ArgumentException("Invalid argument or argument-structure!\nType \"pf.exe help\" to get detailed information about this...");
                    }
                    else
                        throw new ArgumentException("Invalid argument or argument-structure!\nType \"pf.exe help\" to get detailed information about this...");
                }
            } catch (Exception exc)
            {
                Console.WriteLine("\n" + exc.GetType().Name + ":\n" + exc.Message);
                return;
            }
            

            Console.Title = "UPnP Port Forwarding";
        start:

            Console.Clear();
            Console.Write("Your avaiable options:\n" +
                              "\n" +
                              "1: Open port\n" +
                              "2: Close port\n" +
                              "3: List all opened ports\n" +
                              "4: Close every port possible\n" +
                              "q: Exit\n" +
                              "\n" +
                              "Choice: ");

            int ch = 0;
            do
            {
                switch (Console.ReadKey(true).KeyChar)
                {
                    case '1':
                        Console.Write("Open port");
                        ch = 1;
                        break;
                    case '2':
                        Console.Write("Close port");
                        ch = 2;
                        break;
                    case '3':
                        Console.Write("List all opened ports");
                        ch = 3;
                        break;
                    case '4':
                        Console.Write("Close every port possible");
                        ch = 4;
                        break;
                    case 'q':
                        return;
                }
            } while (ch == 0);

            try
            {
                if (ch == 1)
                {
                    Console.Write("\n\nType your port: ");
                    int port;
                    if (!int.TryParse(Console.ReadLine(), out port))
                        throw new ArgumentException("This is no valid number!");
                    else if (port <= 1024 || port > 65535)
                        throw new ArgumentException("The port has to be larger than 1024 and lower than 65536!");
                    var devAsync = new NatDiscoverer().DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource());
                    devAsync.Wait();
                    var dev = devAsync.Result;
                    Console.Write("\nChoose an option:\n1: Open port on TCP\n2: Open port on UDP\nc: Cancel\n\nOption: ");
                    ch = 0;
                    do
                    {
                        switch (Console.ReadKey(true).KeyChar)
                        {
                            case '1':
                                Console.WriteLine("TCP");
                                ch = 1;
                                break;
                            case '2':
                                Console.WriteLine("UDP");
                                ch = 2;
                                break;
                            case 'c':
                                goto start;
                        }
                    } while (ch == 0);
                    Mapping map = new Mapping((Protocol)(ch - 1), port, port);
                    dev.CreatePortMapAsync(map).Wait();
                    Console.WriteLine("\nPort opened!\n\nPress enter to continue...");
                    ConsoleKey k;
                    do
                    {
                        k = Console.ReadKey(true).Key;
                    } while (k != ConsoleKey.Enter);
                    goto start;
                }
                else if (ch == 2)
                {
                    Console.Write("\n\nType your port: ");
                    int port;
                    if (!int.TryParse(Console.ReadLine(), out port))
                        throw new ArgumentException("This is no valid number!");
                    else if (port <= 1024 || port > 65535)
                        throw new ArgumentException("The port has to be larger than 1024 and lower than 65536!");
                    var devAsync = new NatDiscoverer().DiscoverDeviceAsync(PortMapper.Upnp, new CancellationTokenSource());
                    devAsync.Wait();
                    var dev = devAsync.Result;
                    Console.Write("\nChoose an option:\n1: Close port on TCP\n2: Close port on UDP\nc: Cancel\n\nOption: ");
                    ch = 0;
                    do
                    {
                        switch (Console.ReadKey(true).KeyChar)
                        {
                            case '1':
                                Console.WriteLine("TCP");
                                ch = 1;
                                break;
                            case '2':
                                Console.WriteLine("UDP");
                                ch = 2;
                                break;
                            case 'c':
                                goto start;
                        }
                    } while (ch == 0);
                    Mapping map = new Mapping((Protocol)(ch - 1), port, port);
                    dev.DeletePortMapAsync(map);
                    Console.WriteLine("\nPort closed!\n\nPress enter to continue...");
                    ConsoleKey k;
                    do
                    {
                        k = Console.ReadKey(true).Key;
                    } while (k != ConsoleKey.Enter);
                    goto start;
                }
                else if (ch == 3)
                {
                    var devAsync = new NatDiscoverer().DiscoverDeviceAsync();
                    devAsync.Wait();
                    var dev = devAsync.Result;
                    var mapsAsync = dev.GetAllMappingsAsync();
                    mapsAsync.Wait();
                    Console.WriteLine("\n\nEvery opened port:");
                    foreach (Mapping map in mapsAsync.Result)
                        Console.WriteLine("Public: " + map.PublicPort + "; Private: " + map.PrivatePort + "; for device " + map.PrivateIP + "-" + map.Protocol.ToString());
                    Console.WriteLine("\nPress enter to continue...");
                    ConsoleKey k;
                    do
                    {
                        k = Console.ReadKey(true).Key;
                    } while (k != ConsoleKey.Enter);
                    goto start;
                }
                else if (ch == 4)
                {
                    var devAsync = new NatDiscoverer().DiscoverDeviceAsync();
                    devAsync.Wait();
                    var dev = devAsync.Result;
                    var mapsAsync = dev.GetAllMappingsAsync();
                    mapsAsync.Wait();
                    Console.WriteLine("\n\nStart:\n");
                    foreach (Mapping map in mapsAsync.Result)
                    {
                        Task t = dev.DeletePortMapAsync(map);
                        t.Wait();
                        if (t.Exception != null)
                            Console.WriteLine(t.Exception.GetType().Name + ": " + t.Exception.Message + "  (Port " + map.PublicPort + ")");
                        else
                            Console.WriteLine("Port " + map.PublicPort + " closed!");
                    }
                    Console.WriteLine("\nFinished!\n\nPress enter to continue...");
                    ConsoleKey k;
                    do
                    {
                        k = Console.ReadKey(true).Key;
                    } while (k != ConsoleKey.Enter);
                    goto start;
                }

                goto start;
            }
            catch (Exception exc)
            {
                Console.WriteLine("\n" + exc.GetType().Name + ":\n" + exc.Message + "\n\nPress enter to try again...");
                ConsoleKey k;
                do
                {
                    k = Console.ReadKey(true).Key;
                } while (k != ConsoleKey.Enter);
                goto start;
            }
        }
    }
}
