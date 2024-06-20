﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Project1DotNet
{
    internal class Database
    {
        public List<Student> Students { get; }
        private string studentsPath = @"./../../../students.json";
        public List<Subject> Subjects { get; }
        private string subjectsPath = @"./../../../subjects.json";
        public List<Grade> Grades { get; }
        private string gradesPath = @"./../../../grades.json";

        public Database()
        {
            this.Students = ReadStudents();
            this.Subjects = ReadSubjects();
            this.Grades = ReadGrades();
        }

        public void importDatabase()
        {

        }

        // Méthodes "GET"
        public List<Student> GetStudents()
        {
            return this.Students;
        }

        public List<Subject> GetSubjects()
        {
            return this.Subjects;
        }

        public Student GetStudent(int id)
        {
            foreach (Student student in this.Students)
            {
                if (student.Id == id)
                {
                    return student;
                }
            }
            Console.WriteLine(@"/!\ Aucun étudiant ne correspond à cet identifiant.");
            return null;
        }

        public Subject GetSubject(int id)
        {
            foreach (Subject subject in this.Subjects)
            {
                if (subject.Id == id)
                {
                    return subject;
                }
            }
            Console.WriteLine(@"/!\ Aucun cours ne correspond à cet identifiant.");
            return null;
        }

        public List<Grade> GetStudentGrades(int studentId)
        {
            List<Grade> studentGrades = new List<Grade>();
            foreach (Grade grade in this.Grades)
            {
                if (grade.StudentId == studentId)
                {
                    studentGrades.Add(grade);
                }
            }
            return studentGrades;
        }

        // Méthodes "SHOW"
        public void ShowStudents()
        {
            Console.WriteLine("____________________");
            Console.WriteLine("Liste des élèves :");
            foreach (Student student in this.Students)
            {
                Console.Write(student.Id + " ");
                Console.Write(student.FirstName + " ");
                Console.WriteLine(student.LastName);
            }
        }
        public void ShowSubjects()
        {
            Console.WriteLine("____________________");
            Console.WriteLine("Liste des cours :");
            foreach (Subject subject in this.Subjects)
            {
                Console.Write(subject.Id + " ");
                Console.WriteLine(subject.Name);
            }
        }

        public void ShowStudent(int id, Database database)
        {
            foreach (Student student in this.Students)
            {
                if (student.Id == id)
                {
                    Console.WriteLine("--------------------");
                    Console.WriteLine("Informations sur l'élève :");
                    Console.WriteLine("");
                    Console.Write("Identifiant       ");
                    Console.WriteLine($": {student.Id}");
                    Console.Write("Prénom            ");
                    Console.WriteLine($": {student.FirstName}");
                    Console.Write("Nom               ");
                    Console.WriteLine($": {student.LastName}");
                    Console.Write("Date de naissance ");
                    Console.WriteLine($": {student.Birthday.ToString("dd/MM/yyyy")}");
                    Console.WriteLine("");
                    Console.WriteLine("Résultats scolaires :");
                    Console.WriteLine("");
                    student.ShowGrades(student.Id, database);
                    student.ShowAverage(student.Id, database);

                    Console.WriteLine("--------------------");
                    break;
                }
                else if (student.Id == Students.Last().Id)
                {
                    Console.WriteLine(@"/!\ Aucun étudiant ne correspond à cet identifiant.");
                }
            }
        }

        // Méthodes "ADD"
        public void AddStudent(string firstName, string lastName, DateTime birthday)
        {
            try
            {
                this.Students.Add(new Student(GenerateStudentId(), firstName, lastName, birthday));
                WriteStudents();
                Console.WriteLine($"L'étudiant {firstName} {lastName} a bien été ajouté.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"/!\ Une erreur est survenue lors de l'ajout de l'étudiant à la base de données.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void AddGrade(int idStudent, int idSubject, double score, string appreciation)
        {
            try
            {
                Student currentStudent = this.Students[idStudent];
                Subject currentSubject = this.Subjects[idSubject];

                this.Grades.Add(new Grade(idStudent, idSubject, score, appreciation));
                WriteGrades();
                Console.WriteLine($"Un {score}/20 en {currentSubject.Name} a été ajoutée à {currentStudent.FirstName} {currentStudent.LastName}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(@"/!\ Une erreur est survenue lors de l'ajout de la note à la base de données.");
                Console.WriteLine(ex.ToString());
            }
        }

        public void AddSubject(string name)
        {
            try
            {
                this.Subjects.Add(new Subject(GenerateSubjectId(), name));
                WriteSubjects();
                Console.WriteLine($"Le cours {name} a bien été ajouté.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(@"/!\ Une erreur est survenue lors de l'ajout du cours à la base de données.");
                Console.WriteLine(ex.ToString());
            }
        }

        // Méthodes "DELETE"
        public void DeleteSubject(int idSubject)
        {
            try
            {
                //for (int i = 0; i < this.Grades.Count; i++)
                //{
                //    if (Grades[i].SubjectId == idSubject) this.Grades.RemoveAt(i);
                //}

                //for (int i = 0; i < this.Subjects.Count; i++)
                //{
                //    if (Subjects[i].Id == idSubject) this.Subjects.RemoveAt(i);
                //}
                this.Grades.RemoveAll(grade => grade.SubjectId == idSubject);
                this.Subjects.RemoveAll(subject => subject.Id == idSubject);
                WriteSubjects();
                WriteGrades();
                Console.WriteLine($"Le cours ses notes associées ont bien été supprimés.");

            }
            catch (Exception ex)
            {
                Console.WriteLine(@"/!\ Une erreur est survenue lors de la suppression du cours ou des notes associées.");
                Console.WriteLine(ex.ToString());
            }
        }

        // Méthodes "GENERATE"
        public int GenerateStudentId()
        {
            if (Students.Count > 0) return Students.Last().Id + 1;
            else return 0;
        }

        public int GenerateSubjectId()
        {
            if (Subjects.Count > 0) return Subjects.Last().Id + 1;
            else return 0;
        }

        // Méthodes "WRITEFILE"
        public void WriteStudents()
        {
            string json = JsonConvert.SerializeObject(Students, Formatting.Indented);
            File.WriteAllText(studentsPath, json);
        }
        public void WriteSubjects()
        {
            string json = JsonConvert.SerializeObject(Subjects, Formatting.Indented);
            File.WriteAllText(subjectsPath, json);
        }
        public void WriteGrades()
        {
            string json = JsonConvert.SerializeObject(Grades, Formatting.Indented);
            File.WriteAllText(gradesPath, json);
        }

        // Méthodes "READFILE"
        public List<Student> ReadStudents()
        {
            try
            {
                string jsonContent = File.ReadAllText(studentsPath);
                List<Student> importedStudents = JsonConvert.DeserializeObject<List<Student>>(jsonContent);
                return importedStudents;
            }
            catch (Exception ex)
            {
                return new List<Student>();
            }
        }

        public List<Subject> ReadSubjects()
        {
            try
            {
                string jsonContent = File.ReadAllText(subjectsPath);
                List<Subject> importedSubjects = JsonConvert.DeserializeObject<List<Subject>>(jsonContent);
                return importedSubjects;
            }
            catch (Exception ex)
            {
                return new List<Subject>();
            }
        }

        public List<Grade> ReadGrades()
        {
            try
            {
                string jsonContent = File.ReadAllText(gradesPath);
                List<Grade> importedGrades = JsonConvert.DeserializeObject<List<Grade>>(jsonContent);
                return importedGrades;
            }
            catch (Exception ex)
            {
                return new List<Grade>();
            }
        }

    }
}
