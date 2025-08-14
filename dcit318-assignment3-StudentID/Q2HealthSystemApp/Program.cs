
using System;
using System.Collections.Generic;
using System.Linq;

namespace Q2HealthSystemApp
{
    // a) Generic Repository
    public class Repository<T>
    {
        private readonly List<T> items = new();

        public void Add(T item) => items.Add(item);
        public List<T> GetAll() => new(items);
        public T? GetById(Func<T, bool> predicate) => items.FirstOrDefault(predicate);
        public bool Remove(Func<T, bool> predicate)
        {
            var found = items.FirstOrDefault(predicate);
            if (found is null) return false;
            return items.Remove(found);
        }
    }

    // b) Patient
    public class Patient
    {
        public int Id;
        public string Name;
        public int Age;
        public string Gender;

        public Patient(int id, string name, int age, string gender)
        {
            Id = id; Name = name; Age = age; Gender = gender;
        }

        public override string ToString() => $"Patient {{ Id={Id}, Name={Name}, Age={Age}, Gender={Gender} }}";
    }

    // c) Prescription
    public class Prescription
    {
        public int Id;
        public int PatientId;
        public string MedicationName;
        public DateTime DateIssued;

        public Prescription(int id, int patientId, string medicationName, DateTime dateIssued)
        {
            Id = id; PatientId = patientId; MedicationName = medicationName; DateIssued = dateIssued;
        }

        public override string ToString() => $"Rx {{ Id={Id}, PatientId={PatientId}, Med={MedicationName}, Date={DateIssued:d} }}";
    }

    public class HealthSystemApp
    {
        private readonly Repository<Patient> _patientRepo = new();
        private readonly Repository<Prescription> _prescriptionRepo = new();
        private readonly Dictionary<int, List<Prescription>> _prescriptionMap = new();

        public void SeedData()
        {
            _patientRepo.Add(new Patient(1, "Alice Smith", 30, "Female"));
            _patientRepo.Add(new Patient(2, "Bob Johnson", 42, "Male"));
            _patientRepo.Add(new Patient(3, "Cynthia Doe", 28, "Female"));

            _prescriptionRepo.Add(new Prescription(101, 1, "Amoxicillin", DateTime.Today.AddDays(-7)));
            _prescriptionRepo.Add(new Prescription(102, 1, "Ibuprofen", DateTime.Today.AddDays(-2)));
            _prescriptionRepo.Add(new Prescription(103, 2, "Metformin", DateTime.Today.AddDays(-14)));
            _prescriptionRepo.Add(new Prescription(104, 3, "Loratadine", DateTime.Today.AddDays(-1)));
            _prescriptionRepo.Add(new Prescription(105, 2, "Atorvastatin", DateTime.Today));
        }

        public void BuildPrescriptionMap()
        {
            _prescriptionMap.Clear();
            foreach (var rx in _prescriptionRepo.GetAll())
            {
                if (!_prescriptionMap.TryGetValue(rx.PatientId, out var list))
                {
                    list = new List<Prescription>();
                    _prescriptionMap[rx.PatientId] = list;
                }
                list.Add(rx);
            }
        }

        public void PrintAllPatients()
        {
            Console.WriteLine("Patients:");
            foreach (var p in _patientRepo.GetAll())
            {
                Console.WriteLine("  " + p);
            }
        }

        public List<Prescription> GetPrescriptionsByPatientId(int patientId)
        {
            return _prescriptionMap.TryGetValue(patientId, out var list) ? list : new List<Prescription>();
        }

        public void PrintPrescriptionsForPatient(int id)
        {
            var rxs = GetPrescriptionsByPatientId(id);
            Console.WriteLine($"\nPrescriptions for PatientId={id}:");
            if (rxs.Count == 0) Console.WriteLine("  (none)");
            foreach (var rx in rxs) Console.WriteLine("  " + rx);
        }

        public static void Main()
        {
            var app = new HealthSystemApp();
            app.SeedData();
            app.BuildPrescriptionMap();
            app.PrintAllPatients();
            app.PrintPrescriptionsForPatient(2);
        }
    }
}
