using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace StudentRepository
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (IRepository<Student> Students = StudentRepository.Instance)
                {
                    #region Add student 


                    Students.Add(new Student(9) { MobileNo = "01723454570", FirstName = "Farzana", LastName = "Akter", BirthDate = Convert.ToDateTime("01-Jan-1980"), Subject = Subject.WDPF });

                    #endregion



                    var c2 = Students.FindById(2);


                    c2.FirstName = "Hasib";
                    c2.LastName = "Mohammed";


                    Students.Update(c2);


                    Console.WriteLine($"Student{c2.Id} updated successfully");

                    Console.WriteLine(c2.ToString());


                    if (Students.Delete(c2))
                        Console.WriteLine($"Student {c2.Id} deleted successfully");






                    #region Search from repository

                    var data = Students.Search("Akter");
                    Console.WriteLine();
                    Console.WriteLine($"Total Students {data.Count()}");
                    Console.WriteLine("----------------------------------");

                    foreach (var c in data)
                    {
                        Console.WriteLine(c.ToString());

                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                Console.ReadLine();
            }

        }
    }


    public interface IEntity : IDisposable
    {
        int Id { get; }
        bool IsValid();
    }


    public interface IRepository<T> : IDisposable, IEnumerable<T> where T : IEntity
    {

        IEnumerable<T> Data { get; }
        void Add(T entity);
        bool Delete(T entity);
        void Update(T entity);
        T FindById(int Id);
        IEnumerable<T> Search(string value);

    }


    public enum Subject
    {
       CS= 1,
       WDPF = 8,
      NT = 32

    }


    public sealed class Student : IEntity
    {
        public int Id { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get => $"{this.FirstName} {this.LastName}"; }
        public string MobileNo { get; set; }

        public string Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public Subject Subject { get; set; }



        public Student()
        {

        }
        public Student(int StudentId)
        {
            this.Id = StudentId;
            this.BirthDate = null;
            this.Subject = Subject.CS;
        }

        public Student(int StudentId, string MobileNo, string FirstName, string LastName = null, string Email = null, DateTime? BirthDate = null, Subject Subject = Subject.CS)
        {
            this.Id = StudentId;
            this.MobileNo = MobileNo;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
            this.BirthDate = BirthDate;
            this.Subject = Subject;
        }


        public bool IsValid()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(MobileNo))
                isValid = false;
            else if (string.IsNullOrWhiteSpace(FirstName))
                isValid = false;
            else if (string.IsNullOrEmpty(LastName))
                isValid = false;
            else if (this.BirthDate?.Date > DateTime.Now)
                isValid = false;

            return isValid;
        }

        public override string ToString()
        {
            string text = "Student Info\n";
            text = text + $"StudentID : {this.Id}\n";
            text += $"Name  : {this.FullName}\n";
            text += $"Mobile  : {this.MobileNo} \n";
            text += $"Email  : {this.Email} \n";
            text += $"Date of Birth : {this.BirthDate?.ToString("d")}\n";
            text += $"Group : {this.Subject} \n";
            text += $"************************\n";

            return text;
        }
        public void Dispose()
        {

        }
    }


    public sealed class StudentRepository : IRepository<Student>
    {


        private static StudentRepository _instance;
        public static StudentRepository Instance
        {
            get
            {
                return _instance ?? new StudentRepository(); ;
            }
        }

        List<Student> Studentinfo;

        private StudentRepository()
        {
            Studentinfo = new List<Student>
            {
                new Student(StudentId: 1, MobileNo: "1234", "Abdur", "Rahman", "abdur@gmail.com"),
                new Student(StudentId: 2, MobileNo: "01734567787", "Abul", "Kalam", "Kalam@gmail.com"),
                new Student(StudentId: 3, MobileNo: "01757645677", "Abdul", "Momen", "Momen@gmail.com"),
                new Student(StudentId: 4, MobileNo: "", "Jannat", "Akter", "Jannat@gmail.com"),
                new Student(StudentId: 5, MobileNo: "", "Jannatul", "Ferdous", "Ferdous@gmail.com"),
                new Student(StudentId: 6, MobileNo: "", "Abdur", "Rahman", "Rahman@gmail.com"),
                new Student(StudentId: 7, MobileNo: "", "Mohammed", "Mostafa", "Mostafa@gmail.com")
            };

        }
        public void Dispose()
        {
            this.Studentinfo.Clear();
        }


        IEnumerable<Student> IRepository<Student>.Data { get; }


        public Student this[int index]
        {
            get
            {
                return Studentinfo[index];
            }
        }

        public void Add(Student entity)
        {
            if (Studentinfo.Any(c => c.Id == entity.Id))
            {
                throw new Exception("Duplicate Student Id, try another");
            }
            else if (entity.IsValid())
            {
                Studentinfo.Add(entity);
            }
            else
            {
                throw new Exception("Student is invalid");
            }
        }

        public bool Delete(Student entity)
        {
            return Studentinfo.Remove(entity);
        }

        public void Update(Student entity)
        {

            Studentinfo[Studentinfo.FindIndex(c => c.Id == entity.Id)] = entity;

        }

        public Student FindById(int Id)
        {
            var result = (from r in Studentinfo where r.Id == Id select r).FirstOrDefault();
            return result;
        }

        public IEnumerable<Student> Search(string value)
        {

            var result = from r in Studentinfo
                         where
                         r.Id.ToString().Contains(value) ||
                         r.FirstName.StartsWith(value) ||
                         r.LastName.StartsWith(value) ||
                         r.MobileNo.Contains(value) ||
                         r.Email.Contains(value) ||
                         r.BirthDate.ToString().Contains(value)
                         orderby r.FirstName ascending
                         select r;
            return result;
        }

        public IEnumerator<Student> GetEnumerator()
        {
            foreach (var c in Studentinfo)
            {
                yield return c;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var c in Studentinfo)
            {
                yield return c;
            }
        }
    }
}