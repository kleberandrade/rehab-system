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
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PrimeJSON.Core
{
    public class JSONObject : JSONNode
    {
        public Dictionary<string, JSONNode> _value = new Dictionary<string, JSONNode>();
        public string _type = "JSONObject";
        protected const char BEGIN = '{';
        protected const char END = '}';
        protected const string COMMA = ", ";


        public override JSONNode this[string key]
        {
            get
            {
                return this._value[key];
            }
            set
            {
                this._value[key] = value;
            }
        }

        public override JSONNode this[int index]
        {
            get
            {
                JSONNode node = this.ElementAt(index).Value;
                return node;
            }
            set
            {
                string key = this.ElementAt(index).Key;
                this._value[key] = value;
            }
        }

        public override string Type
        {
            get
            {
                return this._type;
            }
        }

        public override int Count
        {
            get
            {
                return this._value.Count;
            }
        }

        public override void Add(string key, JSONNode node)
        {
            this._value.Add(key, node);
        }

        public override JSONNode Remove(int index)
        {
            KeyValuePair<string, JSONNode> kvp = this.ElementAt(index);
            this._value.Remove(kvp.Key);
            return kvp.Value;
        }

        public override JSONNode Remove(string key)
        {
            JSONNode node = this._value[key];
            this._value.Remove(key);
            return node;
        }

        public override JSONNode Remove(JSONNode node)
        {
            string key = this.GetElement(node).Key;
            this._value.Remove(key);
            return node;
        }

        public override IEnumerator GetEnumerator()
        {
            foreach (KeyValuePair<string, JSONNode> kvp in this._value)
                yield return kvp;
        }

        public override string ToJSON()
        {
            StringBuilder json = new StringBuilder("");
            json.Append(JSONObject.BEGIN);
            for (int i = 0; i < this.Count; i++)
            {
                KeyValuePair<string, JSONNode> kvp = this.ElementAt(i);
                json.Append("\"" + kvp.Key + "\" : " + kvp.Value.ToJSON());
                if (i != this.Count - 1)
                    json.Append(JSONObject.COMMA);
            }
            json.Append(JSONObject.END);
            return json.ToString();
        }

        private KeyValuePair<string, JSONNode> ElementAt(int index)
        {
            int i = 0;
            foreach (KeyValuePair<string, JSONNode> kvp in this._value)
            {
                if (i == index)
                    return kvp;
                i++;
            }
            throw new ArgumentOutOfRangeException();
        }

        private KeyValuePair<string, JSONNode> GetElement(JSONNode node)
        {
            foreach (KeyValuePair<string, JSONNode> kvp in this._value)
            {
                if (kvp.Value == node)
                    return kvp;
            }
            throw new ArgumentOutOfRangeException();
        }
    }
}
