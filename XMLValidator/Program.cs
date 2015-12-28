using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace XMLValidator
{
    static class Program
    {
        static void Main(string[] args)
        {
            // Введите имя файла
            load("xmlone.xsd");
            Console.WriteLine(getXML());
            validate();
            Console.ReadKey();
        }

        static Stack stack = new Stack();
        static Boolean flag;
        static String reHead = @"<\?xml version=""1.0""(\s+[a-zA-Z][a-zA-Z0-9]*\s*=\s*""[a-zA-Z][a-zA-Z0-9]*"")*\?>";
        static String tag = @"<[a-zA-Z0-9/=""\s]+>";
        static String reTagOpen = @"<(?<tagName>[a-zA-Z][a-zA-Z0-9]*)(\s+[a-zA-Z][a-zA-Z0-9]*\s*=\s*""[a-zA-Z][a-zA-Z0-9]*"")*>";
        static String reTagEnd = @"</(?<tagName>[a-zA-Z][a-zA-Z0-9]*)>";
        static String xml = "";

        static String getXML() 
        {
            return xml;
        }

        static void load(String fileName)
        {
            try
            {
                using(StreamReader sr = new StreamReader(fileName))
                {
                    while (!sr.EndOfStream)
                    {
                        xml += sr.ReadLine();
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка при чтении: " + e.Message);
            }
        }

        static String printStack()
        {
            String str = "";
            Object[] arr;
            arr = stack.ToArray();
            foreach (Object el in arr)
            {
                str += el.ToString();
                str += ' ';
            }
            return str;
        }

        static void validate()
        {
            // Тег найден
            flag = true;

            try
            {
                // Проверка заголовка файла xml
                Regex regHead = new Regex(reHead);
               Match matchHead = regHead.Match(xml);
                if (!matchHead.Success) throw new Exception("Ошибка в заголовке");

                // Пока найден тег
                Regex regTag = new Regex(tag);
                Match matchTag = regTag.Match(xml);
                while(matchTag.Success)
                {
                    if(String.IsNullOrEmpty(matchTag.Value.ToString())) break;
                    // Если открывающий
                    Regex regOpen = new Regex(reTagOpen);
                    Match matchOpen = regOpen.Match(matchTag.Value);
                    if (matchOpen.Success)
                    {
                        // Кладем в стек
                        stack.Push(matchOpen.Groups["tagName"].Value);
                    }

                    // Если закрывающий
                    Regex regEnd = new Regex(reTagEnd);
                    Match matchEnd = regEnd.Match(matchTag.Value);
                    if (matchEnd.Success)
                    {
                        if (stack.Count != 0)
                        {
                            String openTag = (String)stack.Peek();
                            if (openTag.Equals(matchEnd.Groups["tagName"].Value))
                            {
                                stack.Pop();
                            }
                            else
                            {
                                throw new Exception("Закрывающий тег " + matchEnd.Groups["tagName"].Value + " не равен открывающему " + openTag);
                            }
                        }
                        else
                        {
                            throw new Exception("У тега" + matchEnd.Groups["tagName"].Value + "нет открывающего");
                        }
                    }
                    if (!matchEnd.Success && !matchOpen.Success) throw new Exception("Тег не верен");
                    matchTag = matchTag.NextMatch();
                }


                    // Пуст ли стек
                    if (stack.Count != 0)
                    {
                        throw new Exception("Не все теги закрыты, а именно:\n" + printStack());
                    }


                Console.WriteLine("Файл валиден! Поздравляем");
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка: " + e.Message);
            }
            
        } // Validate
    } // Class
} // Namespace