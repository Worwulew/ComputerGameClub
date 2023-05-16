using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerGameClub
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ComputerClub computerClub = new ComputerClub(9);
            computerClub.Work();

        }
    }

    class ComputerClub
    {
        private int _money = 0;
        private List<Computer> _computers = new List<Computer>();
        private Queue<Client> _clients = new Queue<Client>();

        public ComputerClub(int computersCount)
        {
            Random random = new Random();

            for (int i = 0; i < computersCount; i++)
            {
                _computers.Add(new Computer(random.Next(5, 16)));
            }

            CreateNewClients(25, random);
        }

        public void CreateNewClients(int count, Random random)
        {
            for (int i = 0; i < count; i++)
            {
                _clients.Enqueue(new Client(random.Next(100, 251), random));
            }
        }

        public void Work()
        {
            while (_clients.Count > 0)
            {
                Client newClient = _clients.Dequeue();

                Console.WriteLine($"The computer club balance: {_money}$. Waiting for a new client.");
                Console.WriteLine($"You have a new client, they want to buy {newClient.DesiredMinutes} minutes.");
                ShowAllComputersStates();

                Console.Write("\nYou propose them computer under the number: ");
                string userInput = Console.ReadLine();

                if (int.TryParse(userInput, out int compNumber))
                {
                    compNumber -= 1;

                    if (compNumber >= 0 && compNumber <= _computers.Count)
                    {
                        if (_computers[compNumber].Taken)
                        {
                            Console.WriteLine("This computer is not available right now.");
                        }
                        else
                        {
                            if (newClient.CheckSolvensy(_computers[compNumber]))
                            {
                                Console.WriteLine("Computer time bought successfully! Client can take a seat at computer №" + (compNumber + 1) + "!");
                                _money += newClient.Pay();
                                _computers[compNumber].BecomeTaken(newClient);
                            }
                            else
                            {
                                Console.WriteLine("Client doesn't have enough money.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("There's no such computer number.");
                    }
                }
                else
                {
                    CreateNewClients(1, new Random());
                    Console.WriteLine("Wrong input!");
                }

                Console.WriteLine("Tap any key to turn to another client...");
                Console.ReadKey();
                Console.Clear();
                SpendOneMinute();
            }
        }

        private void ShowAllComputersStates()
        {
            Console.WriteLine("\nThe list of all computers:");
            for (int i = 0; i < _computers.Count; i++)
            {
                Console.Write(i + 1 + " - ");
                _computers[i].ShowState();
            }
        }

        private void SpendOneMinute()
        {
            foreach (var computer in _computers)
            {
                computer.SpendOneMinute();
            }
        }
    }

    class Computer
    {
        private Client _client;
        private int _minutesRemaining;
        public bool Taken
        {
            get { return _minutesRemaining > 0; }
        }

        public int PricePerMinute { get; private set; }

        public Computer(int pricePerMinute)
        {
            PricePerMinute = pricePerMinute;
        }

        public void BecomeTaken(Client client)
        {
            _client = client;
            _minutesRemaining = _client.DesiredMinutes;
        }

        public void BecomeEmpty()
        {
            _client = null;
        }

        public void SpendOneMinute()
        {
            _minutesRemaining--;
        }

        public void ShowState()
        {
            if (Taken)
            {
                Console.WriteLine($"Computer is taken. minutes remaining: {_minutesRemaining}");
            }
            else
            {
                Console.WriteLine($"Computer is available, price per minute: {PricePerMinute}");
            }
        }

    }

    class Client
    {
        private int _money;
        private int _moneyToPay;
        public int DesiredMinutes { get; private set; }

        public Client(int money, Random random)
        {
            _money = money;
            DesiredMinutes = random.Next(10, 31);
        }

        public bool CheckSolvensy(Computer computer)
        {
            _moneyToPay = DesiredMinutes * computer.PricePerMinute;
            if (_money > _moneyToPay)
            {
                return true;
            }
            else
            {
                _moneyToPay = 0;
                return false;
            }
        }

        public int Pay()
        {
            _money -= _moneyToPay;
            return _moneyToPay;
        }
    }
}