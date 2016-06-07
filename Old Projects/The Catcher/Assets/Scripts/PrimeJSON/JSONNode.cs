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
    public abstract class JSONNode : IEnumerable
    {
        // Public Interface
        public virtual string Type { get { throw new NotImplementedException(); } }
        public virtual string ToJSON() { throw new NotImplementedException(); }

        // JSONArray and JSONObject Interface
        public virtual void Add(JSONNode node) { throw new NotImplementedException(); }
        public virtual void Add(string key, JSONNode node) { throw new NotImplementedException(); }
        public virtual JSONNode this[int index] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual JSONNode this[string key] { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual int Count { get { throw new NotImplementedException(); } }
        public virtual JSONNode Remove(string key) { throw new NotImplementedException(); }
        public virtual JSONNode Remove(int index) { throw new NotImplementedException(); }
        public virtual JSONNode Remove(JSONNode node) { throw new NotImplementedException(); }
        public virtual IEnumerator GetEnumerator() { throw new NotImplementedException(); }
       
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // JSONData Interface
        public virtual string JSONString { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual bool JSONBoolean { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
        public virtual double JSONNumber { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }
    }
}
