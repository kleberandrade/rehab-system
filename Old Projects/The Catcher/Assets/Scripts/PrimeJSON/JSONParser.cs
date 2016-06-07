/* The MIT License (MIT)
 * 
 * Copyright (c) 2013 PrimeJSON
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace PrimeJSON.Core
{
    public class JSONParser
    {
        public static JSONNode Parse(string json)
        {
            int i = 0;
            if (json[i] == '{')
                return JSONParser.automatoObject(json, ref i);
            else if (json[i] == '[')
                return JSONParser.automatoArray(json, ref i);
            else
                throw new FormatException();
        }

        private static string automatoString(string str, ref int index)
        {
            int estado = 0;
            StringBuilder valor = new StringBuilder();

            while (index < str.Length)
            {
                if (str[index] == '"' && estado == 0)
                    estado = 1;
                else if (str[index] == '"' && estado == 1)
                    break;
                else if (estado == 1)
                    valor.Append(str[index]);
                index++;
            }
            return valor.ToString();
        }

        private static bool automatoTrue(string str, ref int index)
        {
            int estado = 0;
            StringBuilder valor = new StringBuilder();

            while (index < str.Length)
            {
                if (str[index] == 't' && estado == 0)
                {
                    valor.Append(str[index]);
                    estado = 1;
                }
                else if (str[index] != 't' && estado == 0)
                    throw new FormatException();
                else if (str[index] == 'r' && estado == 1)
                {
                    valor.Append(str[index]);
                    estado = 2;
                }
                else if (str[index] != 'r' && estado == 1)
                    throw new FormatException();
                else if (str[index] == 'u' && estado == 2)
                {
                    valor.Append(str[index]);
                    estado = 3;
                }
                else if (str[index] != 'u' && estado == 2)
                    throw new FormatException();
                else if (str[index] == 'e' && estado == 3)
                {
                    valor.Append(str[index]);
                    break;
                }
                else if (str[index] != 'e' && estado == 3)
                    throw new FormatException();
                else
                    estado = 0;
                index++;
            }

            return Convert.ToBoolean(valor.ToString());
        }

        private static bool automatoFalse(string str, ref int index)
        {
            int estado = 0;
            StringBuilder valor = new StringBuilder();

            while (index < str.Length)
            {
                if (str[index] == 'f' && estado == 0)
                {
                    valor.Append(str[index]);
                    estado = 1;
                }
                else if (str[index] != 'f' && estado == 0)
                    throw new FormatException();
                else if (str[index] == 'a' && estado == 1)
                {
                    valor.Append(str[index]);
                    estado = 2;
                }
                else if (str[index] != 'a' && estado == 1)
                    throw new FormatException();
                else if (str[index] == 'l' && estado == 2)
                {
                    valor.Append(str[index]);
                    estado = 3;
                }
                else if (str[index] != 'l' && estado == 2)
                    throw new FormatException();
                else if (str[index] == 's' && estado == 3)
                {
                    valor.Append(str[index]);
                    estado = 4;
                }
                else if (str[index] != 's' && estado == 3)
                    throw new FormatException();
                else if (str[index] == 'e' && estado == 4)
                {
                    valor.Append(str[index]);
                    break;
                }
                else if (str[index] != 'e' && estado == 4)
                    throw new FormatException();
                else
                    estado = 0;
                index++;
            }

            return Convert.ToBoolean(valor.ToString());
        }

        private static string automatoNull(string str, ref int index)
        {
            int estado = 0;
            StringBuilder valor = new StringBuilder();

            while (index < str.Length)
            {
                if (str[index] == 'n' && estado == 0)
                {
                    valor.Append(str[index]);
                    estado = 1;
                }
                else if (str[index] != 'n' && estado == 0)
                    throw new FormatException();
                else if (str[index] == 'u' && estado == 1)
                {
                    valor.Append(str[index]);
                    estado = 2;
                }
                else if (str[index] != 'u' && estado == 1)
                    throw new FormatException();
                else if (str[index] == 'l' && estado == 2)
                {
                    valor.Append(str[index]);
                    estado = 3;
                }
                else if (str[index] != 'l' && estado == 2)
                    throw new FormatException();
                else if (str[index] == 'l' && estado == 3)
                {
                    valor.Append(str[index]);
                    break;
                }
                else if (str[index] != 'l' && estado == 3)
                    throw new FormatException();
                else
                {
                    estado = 0;
                }
                index++;
            }

            if (valor.ToString() == "null")
                return null;
            else
                throw new FormatException();
        }

        private static double automatoNumber(string str, ref int index)
        {
            int estado = 0;
            StringBuilder valor = new StringBuilder();

            while (index < str.Length)
            {
                if (str[index] == '-' && estado == 0)
                {
                    valor.Append(str[index]);
                    estado = 1;
                }
                else if (str[index] == '0' && (estado == 0 || estado == 1))
                {
                    valor.Append(str[index]);
                    estado = 2;
                }
                else if ((Char.GetNumericValue(str[index]) > 0 && Char.GetNumericValue(str[index]) < 10) && (estado == 0 || estado == 1))
                {
                    valor.Append(str[index]);
                    estado = 3;
                }
                else if ((str[index] == 'e' || str[index] == 'E') && (estado == 2 || estado == 3 || estado == 4))
                {
                    valor.Append(str[index]);
                    estado = 6;
                }
                else if (str[index] == '+' || str[index] == '-' && estado == 6)
                {
                    valor.Append(str[index]);
                    estado = 7;
                }
                else if (str[index] == '.' && (estado == 2 || estado == 3 || estado == 4))
                {
                    valor.Append(str[index]);
                    estado = 5;
                }
                else if ((Char.GetNumericValue(str[index]) >= 0 && Char.GetNumericValue(str[index]) <= 9) && (estado == 3 || estado == 4 || estado == 5 || estado == 6 || estado == 7))
                {
                    valor.Append(str[index]);
                    estado = 4;
                }
                else if ((str[index] == ' ' || str[index] == ']' || str[index] == '}' || str[index] == ',') && (estado == 2 || estado == 3 || estado == 4))
                    break;
                else if (str[index] != '-' && !char.IsNumber(str[index]) && estado == 0)
                    throw new FormatException();
                else if (!char.IsNumber(str[index]) && estado == 1)
                    throw new FormatException();
                else if (str[index] != 'e' && str[index] != 'E' && str[index] != '.' && estado == 2)
                    throw new FormatException();
                else if (str[index] != 'e' && str[index] != 'E' && str[index] != '.' && !char.IsNumber(str[index]) && (estado == 3 || estado == 4))
                    throw new FormatException();
                else if (!char.IsNumber(str[index]) && (estado == 5 || estado == 7))
                    throw new FormatException();
                else if (str[index] != '+' && str[index] != '-' && !char.IsNumber(str[index])  && estado == 6)
                    throw new FormatException();
                index++;
            }
            index--;
            return double.Parse(valor.ToString(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture);
        }

        private static JSONNode automatoArray(string str, ref int index)
        {
            JSONNode json = new JSONArray();
            int estado = 0;

            while (index < str.Length)
            {
                if ((str[index] == '[' || str[index] == ',') && estado == 0)
                    estado = 1;
                else if (str[index] == '"' && estado == 1)
                {
                    json.Add(new JSONData(JSONParser.automatoString(str, ref index)));
                    estado = 0;
                }
                else if (str[index] == 't' && estado == 1)
                {
                    json.Add(new JSONData(JSONParser.automatoTrue(str, ref index)));
                    estado = 0;
                }
                else if (str[index] == 'f' && estado == 1)
                {
                    json.Add(new JSONData(JSONParser.automatoFalse(str, ref index)));
                    estado = 0;
                }
                else if (str[index] == 'n' && estado == 1)
                {
                    json.Add(new JSONData(automatoNull(str, ref index)));
                    estado = 0;
                }
                else if ((char.IsNumber(str[index]) || str[index] == '-') && estado == 1)
                {
                    json.Add(new JSONData(JSONParser.automatoNumber(str, ref index)));
                    estado = 0;
                }
                else if (str[index] == '[' && estado == 1)
                {
                    json.Add(JSONParser.automatoArray(str, ref index));
                    estado = 0;
                }
                else if (str[index] == '{' && estado == 1)
                    json.Add(JSONParser.automatoObject(str, ref index));
                else if (str[index] == ']' && estado == 0)
                    break;
                else if (str[index] == ' ') { }
                else
                    throw new FormatException();
               index++;
            }

            return json;
        }

        private static JSONNode automatoObject(string str, ref int index)
        {
            JSONNode json = new JSONObject();
            int estado = 0;
            string key = "";

            while (index < str.Length)
            {
                if ((str[index] == '{' || str[index] == ',') && estado == 0)
                    estado = 1;
                else if (str[index] == ':' && estado == 0)
                    estado = 2;
                else if (str[index] == '"' && (estado == 1 || estado == 2))
                {
                    if (estado == 1)
                        key = JSONParser.automatoString(str, ref index);
                    else if (estado == 2)
                    {
                        json.Add(key, new JSONData(JSONParser.automatoString(str, ref index)));
                        key = "";
                    }
                    estado = 0;
                }
                else if (str[index] == 't' && estado == 2)
                {
                    json.Add(key, new JSONData(JSONParser.automatoTrue(str, ref index)));
                    key = "";
                    estado = 0;
                }
                else if (str[index] == 'f' && estado == 2)
                {
                    json.Add(key, new JSONData(JSONParser.automatoFalse(str, ref index)));
                    key = "";
                    estado = 0;
                }
                else if (str[index] == 'n' && estado == 2)
                {
                    json.Add(key, new JSONData(JSONParser.automatoNull(str, ref index)));
                    key = "";
                    estado = 0;
                }
                else if (((Char.GetNumericValue(str[index]) >= 0 && Char.GetNumericValue(str[index]) <= 9) || str[index] == '-') && estado == 2)
                {
                    json.Add(key, new JSONData(JSONParser.automatoNumber(str, ref index)));
                    key = "";
                    estado = 0;
                }
                else if (str[index] == '[' && estado == 2)
                {
                    json.Add(key, JSONParser.automatoArray(str, ref index));
                    key = "";
                    estado = 0;
                }
                else if (str[index] == '{' && estado == 2)
                {
                    json.Add(key, JSONParser.automatoObject(str, ref index));
                    key = "";
                    estado = 0;
                }
                else if (str[index] == '}' && estado == 0)
                    break;
                else if (str[index] == ' ') { }
                else
                    throw new FormatException();
                index++;
            }
            return json;
        }
    }
}
