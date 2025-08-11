using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthSystemApp
{
    // a. Generic Repository<T>
    public class Repository<T>
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);

        public List<T> GetAll() => new List<T>(items);

        public T? GetById(Func<T, bool> predicate)
            => items.FirstOrDefault(predicate);

        public bool Remove(Func<T, bool> predicate)
        {
            var item = items.FirstOrDefault(predicate);
            if (item != null)
            {
                items.Remove(item);
                return true;
            }
            return false;
        }
    }

    // b. Patient Class
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id;
            Name = name;
            Age = age;
            Gender = gender;
        }
    }

    // c. Prescription Class
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id;
            PatientId = patientId;
            MedicationName = medicationName;
            DateIssued = dateIssued;
        }
    }

    // d–g. HealthSystemApp Class
    public class HealthSystemApp
    {
        private Repository<Patient> _patientRepo = new Repository<Patient>();
        private Repository<Prescription> _prescriptionRepo = new Repository<Prescription>();
        private Dictionary<int, List<Prescription>> _prescriptionMap = new Dictionary<int, List<Prescription>>();

        public void SeedData()
        {
            // Add patients
            _patientRepo.Add(new Patient(1, "Ama Owusu", 28, "Female"));
            _patientRepo.Add(new Patient(2, "Kwame Mensah", 35, "Male"));
            _patientRepo.Add(new Patient(3, "Akua Asare", 42, "Female"));

            // Add prescriptions
            _prescriptionRepo.Add(new Prescription(1, 1, "Paracetamol", DateTime.Now.AddDays(-5)));
            _prescriptionRepo.Add(new Prescription(2, 1, "Ibuprofen", DateTime.Now.AddDays(-3)));
            _prescriptionRepo.Add(new Prescription(3, 2, "Amoxicillin", DateTime.Now.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(4, 3, "Vitamin C", DateTime.Now.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(5, 3, "Cough Syrup", DateTime.Now));
        }

        public void BuildPrescriptionMap()
        {
            foreach (var prescription in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.ContainsKey(prescription.PatientId))
                {
                    _prescriptionMap[prescription.PatientId] = new List<Prescription>();
                }

                _prescriptionMap[prescription.PatientId].Add(prescription);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("List of Patients:");
            foreach (var patient in _patientRepo.GetAll())
            {
                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Age: {patient.Age}, Gender: {patient.Gender}");
            }
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            if (_prescriptionMap.ContainsKey(id))
            {
                Console.WriteLine($"\nPrescriptions for Patient ID {id}:");
                foreach (var p in _prescriptionMap[id])
                {
                    Console.WriteLine($"- {p.MedicationName} (Issued: {p.DateIssued.ToShortDateString()})");
                }
            }
            else
            {
                Console.WriteLine("No prescriptions found for that patient.");
            }
        }
    }

    // Main method
    class Program
    {
        static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();

            Console.Write("\nEnter a Patient ID to view prescriptions: ");
            if (int.TryParse(Console.ReadLine(), out int patientId))
            {
                app.PrintPrescriptionsForPatient(patientId);
            }
            else
            {
                Console.WriteLine("Invalid input.");
            }
        }
    }
}
