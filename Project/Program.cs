using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Assignment_8
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
             *  Note that Courses.csv is a folder named App_Data, this folder is located at
             *      bin\Debug\netcoreapp3.1
             *  in the program's folder, this is because that is where it defaults to read from.
             *  
             *  Courses.xml and Instructors.csv are there as well
             */

            //2.1
            var filepath = @"App_Data\\Courses.csv";
            var reader = new StreamReader(filepath);

            //Begin reading the Courses.csv file
            String line = reader.ReadLine();    //first line won't hold any info we want
            if(line != null)
            {
                line = reader.ReadLine();   //get the second line
            }

            /*
             * 2.1 said to create an XML file using the Course class that we create, but this would require serializing the class
             * and that makes reading/writing to a single XML file a huge pain as each searialization creates a new XML file, also
             * the lecture slides never mentioned anything about doing this, so I assume it means that we are supposed to do this
             * like in the lecture slides by using XElement to create the XML file in-memory
             */

            //create the root element
            XElement courses = new XElement("Courses");

            while (line != null)
            {
                String[] columns = line.Split(','); //split the line to get the info

                //new Courses object
                Course newCourse = new Course();
                newCourse.CourseId = columns[2];
                newCourse.Subject = columns[0];
                newCourse.CourseCode = columns[1];
                newCourse.Location = columns[7];
                newCourse.Instructor = columns[10];
                newCourse.Title = columns[3];

                //create a child element and poulate it with info
                XElement course = new XElement("Course");
                course.Add(new XElement("CourseID", newCourse.CourseId));
                course.Add(new XElement("Subject", newCourse.Subject));
                course.Add(new XElement("CourseCode", newCourse.CourseCode));
                course.Add(new XElement("Location", newCourse.Location));
                course.Add(new XElement("Instructor", newCourse.Instructor));
                course.Add(new XElement("Title", newCourse.Title));

                //add the child to the root
                courses.Add(course);

                line = reader.ReadLine();   //read next line
            }

            reader.Close(); //close the file reader

            //2.2
            courses.Save(@"App_Data\\Courses.xml"); //save the xml file to App_Data\Courses.xml

            //2.3 A
            IEnumerable<XElement> courses200 =
                from elem in courses.Elements("Course")
                where Int32.Parse((String)elem.Element("CourseCode")) >= 200
                let title = (String)elem.Element("Title")
                let inst = (String)elem.Element("Instructor")
                orderby (String)elem.Element("Instructor")
                select new XElement("Course", new XElement("Title", title), new XElement("Instructor", inst));

            Console.WriteLine("-----------------------------------2.3 A-----------------------------------");

            foreach (var elem in courses200)
            {
                Console.WriteLine((String)elem.Element("Title") +"\t"+ (String)elem.Element("Instructor"));
            }

            //2.3 B
            var data = 
                from elem in courses.Elements("Course")
                select new {CourseId = (String)elem.Element("CourseId"),
                            Subject = (String)elem.Element("Subject"),
                            CourseCode = (String)elem.Element("CourseCode"),
                            Location = (String)elem.Element("Location"),
                            Instructor = (String)elem.Element("Instructor"),
                            Title = (String)elem.Element("Title")};

            var groupedData =
                from d in data
                group d by d.Subject into group1
                from group2 in
                    (from d in group1
                     group d by d.CourseCode)
                group group2 by group1.Key;

            Console.WriteLine();
            Console.WriteLine("-----------------------------------2.3 B-----------------------------------");

            foreach(var outer in groupedData)
            {
                if(outer.Count() >= 2)
                {
                    Console.WriteLine($"Class Subject:\t{outer.Key}");
                    foreach (var inner in outer)
                    {
                        Console.WriteLine($"\tClass Number:\t{inner.Key}");
                    }
                    Console.WriteLine();
                }
            }

            //2.4
            List<String> instructors = new List<String>();
            filepath = @"App_Data\\Instructors.csv";
            reader = new StreamReader(filepath);

            line = reader.ReadLine();   //the first line will hold no relevant info
            if (line != null)
            {
                line = reader.ReadLine();  //get the second line
            }

            while (line != null)
            {
                String[] columns = line.Split(','); //length of 3

                //create a new Instructor string using the data from the Instructors.csv file
                String newIns = columns[0];
                String newEmail = columns[2];

                //add it to the instructor list
                instructors.Add(newIns);
                instructors.Add(newEmail);

                line = reader.ReadLine();
            }

            IEnumerable<XElement> courseEmail =
                from elem in courses.Elements("Course")
                where Int32.Parse((String)elem.Element("CourseCode")) >= 200
                where Int32.Parse((String)elem.Element("CourseCode")) < 300
                orderby (String)elem.Element("CourseCode")
                let sub = (String)elem.Element("Subject")
                let code = (String)elem.Element("CourseCode")
                let ins = (String)elem.Element("Instructor")
                from i in instructors
                where i == ins
                let email = instructors[instructors.IndexOf(i) + 1]
                select new XElement("Course", new XElement("Subject", sub), new XElement("CourseCode", code), new XElement("Email", email));

            Console.WriteLine("-----------------------------------2.4-----------------------------------");

            foreach(var course in courseEmail)
            {
                Console.WriteLine((String)course.Element("Subject") +"\t"+ (String)course.Element("CourseCode") + "\t" + (String)course.Element("Email"));
            }

            reader.Close();
        }
    }

    //2.1
    public class Course
    {
        public String CourseId, Subject, CourseCode, Location, Instructor, Title;

        public Course()
        {
            this.CourseId = "";
            this.Subject = "";
            this.CourseCode = "";
            this.Location = "";
            this.Instructor = "";
        }
    }
}
