using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

class VechicleInformation
{
    public VechicleInformation() { }
    public VechicleInformation(string vehicleNo, uint age)
    {
        RegistrationNumber = vehicleNo;
        DriverAge = age;
    }
    public string RegistrationNumber { get; set; }
    public uint DriverAge { get; set; }

    public void ParkVehicleInSlot(string vehicleNo, uint age)
    {
        this.RegistrationNumber = vehicleNo;
        this.DriverAge = age;
    }
    public void MarkSlotAsEmpty()
    {
        RegistrationNumber = "";
        DriverAge = 0;
    }
}


class Program
{
    public static void Main(string[] args)
    {
        var InputFileLines = File.ReadAllLines(@"input.txt");
        var firstLine = InputFileLines.First().Split(" ");
        int.TryParse(firstLine[1], out int parkingLotSizeFromFile);
        ParkingSLot parking = new ParkingSLot(parkingLotSizeFromFile);

        foreach (string line in InputFileLines)
        {
            var input = line.Split(" ");
            uint age;
            switch (input[0].ToLower())
            {
                case "park":
                    uint.TryParse(input[3], out age);
                    parking.ParkCar(input[1], age);
                    break;
                case "slot_numbers_for_driver_of_age":
                    uint.TryParse(input[1], out age);
                    parking.SlotNumbersForDriverOfAge(age);
                    break;
                case "slot_number_for_car_with_number":
                    parking.SlotNumbersForCarOfNumber(input[1]);
                    break;
                case "vehicle_registration_number_for_driver_of_age":
                    uint.TryParse(input[1], out age);
                    parking.VehcileNumbersForDriverOfAge(age);
                    break;
                case "leave":
                    int.TryParse(input[1], out int slot);
                    parking.LeaveSlot(slot);
                    break;
                default:
                    break;
            }
        }
        /*int counter = 0;
        foreach (string line in File.ReadLines(@"input.txt"))
        {
            System.Console.WriteLine(line);
            counter++;
        }

        System.Console.WriteLine("There were {0} lines.", counter);

        int n = 6;
        ParkingSLot parking = new ParkingSLot(n);

        parking.ParkCar("KA-01-HH-1234", 21);
        parking.ParkCar("PB-01-HH-1234", 21);
        parking.SlotNumbersForDriverOfAge(21);
        parking.ParkCar("PB-01-TG-2341", 40);
        parking.SlotNumbersForCarOfNumber("PB-01-HH-1234");
        parking.SlotNumbersForCarOfNumber("PB-01-TG-2341");
        parking.LeaveSlot(2);
        parking.ParkCar("HR-29-TG-3098", 39);
        parking.SlotNumbersForCarOfNumber("HR-29-TG-3098");
        parking.SlotNumbersForCarOfNumber("PB-01-TG-2341");
        parking.SlotNumbersForDriverOfAge(18);
        parking.SlotNumbersForCarOfNumber("KA-01-HH-1234");
        parking.LeaveSlot(1);
        parking.SlotNumbersForCarOfNumber("KA-01-HH-1234");
        parking.ParkCar("KA-01-HH-1234", 21);
        parking.ParkCar("PB-01-HH-1234", 21);
        parking.VehcileNumbersForDriverOfAge(21);*/
    }
}

class ParkingSLot
{
    public List<VechicleInformation> _parkingArray;
    PriorityQueue<int, int> _availablityQueue;
    readonly int _parkingSize;

    public ParkingSLot(int Size)
    {
        _parkingSize = Size;
        CreateSlots(Size);
        MarkAllSpotsAvailable(Size);
    }

    private void CreateSlots(int Size)
    {
        _parkingArray = new List<VechicleInformation>(Size);

        for (int i = 1; i <= Size; i++)
        {
            _parkingArray.Add(new VechicleInformation());
        }
        Console.WriteLine($"Created parking of {Size} slots");
    }
    private void MarkAllSpotsAvailable(int Size)
    {
        _availablityQueue = new PriorityQueue<int, int>(Size);
        for (int i = 1; i <= Size; i++)
        {
            _availablityQueue.Enqueue(i - 1, i);
        }
    }

    public void ParkCar(string vehicleNo, uint age)
    {
        if (_availablityQueue.Count == 0)
        {
            Console.WriteLine("Parking is Full, No Space Left!!");
            return; // No Space Left
        }

        var firstAvailableSlot = _availablityQueue.Dequeue();
        _parkingArray[firstAvailableSlot].ParkVehicleInSlot(vehicleNo, age);
        Console.WriteLine($"Car with vehicle registration number \"{vehicleNo}\" has been parked at slot number {firstAvailableSlot}");
    }

    public void SlotNumbersForDriverOfAge(uint age)
    {
        var slotNos = _parkingArray.Select((obj, i) => new { DriverAge = obj.DriverAge, index = i })
                            .Where(x => x.DriverAge == age).Select(x => x.index).ToList();

        Console.Write($"Slot Nos for Age {age} are:- ");
        foreach (var i in slotNos)
            Console.Write((i + 1) + ", ");
        Console.WriteLine();
    }

    public void SlotNumbersForCarOfNumber(string vehicleNumber)
    {
        var vehicleInfo = _parkingArray.FirstOrDefault(x => x.RegistrationNumber == vehicleNumber);
        var slotNo = _parkingArray.IndexOf(vehicleInfo) + 1;

        var ans = string.Concat("SlotNo of car ", vehicleNumber, " is ", slotNo);

        Console.WriteLine(ans);
    }

    public void LeaveSlot(int SlotNo)
    {
        SlotNo -= 1;
        Console.WriteLine($"Slot number {SlotNo + 1} vacated, the car with vehicle registration number \"{_parkingArray[SlotNo].RegistrationNumber}\" left the space, the driver of the car was of age {_parkingArray[SlotNo].DriverAge}");
        _parkingArray[SlotNo].MarkSlotAsEmpty();
        _availablityQueue.Enqueue(SlotNo, SlotNo);
    }

    public void VehcileNumbersForDriverOfAge(uint age)
    {
        var vehicles = _parkingArray.Where(x => x.DriverAge == age).Select(x => x.RegistrationNumber).ToList();

        Console.Write($"Vehcile Nos for Age {age} are:- ");
        foreach (var vehicle in vehicles)
            Console.Write(vehicle + ", ");
        Console.WriteLine();
    }
}