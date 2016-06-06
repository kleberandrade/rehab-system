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
    public class JSONArray : JSONNode
    {
        public List<JSONNode> _value = new List<JSONNode>();
        private string _type = "JSONArray";
        protected const char BEGIN = '[';
        protected const char END = ']';
        protected const string COMMA = ", ";

        public override JSONNode this[int index]
        {
            get
            {
                return this._value[index];
            }
            set
            {
                this._value[index] = value;
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

        public override void Add(JSONNode node)
        {
            this._value.Add(node);
        }

        public override JSONNode Remove(int index)
        {
            JSONNode node = this._value[index];
            this._value.RemoveAt(index);
            return node;
        }

        public override JSONNode Remove(JSONNode node)
        {
            this._value.Remove(node);
            return node;
        }

        public override IEnumerator GetEnumerator()
        {
            foreach (JSONNode node in this._value)
                yield return node;
        }

        public override string ToJSON()
        {
            StringBuilder json = new StringBuilder("");
            json.Append(JSONArray.BEGIN);
            for (int i = 0; i < this.Count; i++)
            {
                json.Append(this[i].ToJSON());
                if (i != this.Count - 1)
                    json.Append(JSONArray.COMMA);
            }
            json.Append(JSONArray.END);
            return json.ToString();
        }
    }
}
