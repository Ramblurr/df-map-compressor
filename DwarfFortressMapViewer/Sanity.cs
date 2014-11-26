/*
Copyright (c) 2003-2004, Richard Dillingham
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    * Neither the name of the copyright holder nor the names of contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDER(S) AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

(For the curious, this is a BSD-style license)
*/

//Version: 2004-Nov-12-Planets

namespace SL.Automation {
	using System;
	using System.Diagnostics;
	
	public class SanityCheckException : System.ApplicationException {
		public SanityCheckException() {
		}
		public SanityCheckException(string s) : base(s) {
		}
	}
	
	//Sanity.IfTrueThrow sounds better than Debug.Assert, methinks. Besides, this throws a SanityCheckException, so it's
	//quite obvious what kind of problem it is - something which we didn't want to happen, happened!
	public class Sanity {
		
		[Conditional("DEBUG")]
		public static void WriteInfo(bool b, string s) {
			if (b) {
				Console.WriteLine("Info: "+s);
			}
		}
		
		[Conditional("DEBUG")]
		public static void IfTrueThrow(bool b, string s) {
			if (b) {
				Throw(s);
			}
		}
		//Sanity.IfTrueThrow sounds better than Debug.Assert, methinks. This throws a SanityCheckException.
		[Conditional("DEBUG")]
		public static void Throw(string s) {
			throw new SanityCheckException(s);
		}
		
		[Conditional("DEBUG")]
		public static void IfTrueSay(bool b, string s) {
			if (b) {
				Say(s);
			}
		}
		
		//This writes an error message and a stack DEBUG, but continues running.
		[Conditional("DEBUG")]
		public static void Say(string s) {
			Console.WriteLine("ERROR: "+s);
			StackTrace();
		}
		
		
		[Conditional("DEBUG")]
		public static void StackTrace() {
			Console.WriteLine("Stack DEBUG:");
			Console.WriteLine(Environment.StackTrace);
			/*StackTrace st = new StackTrace(true);
			for(int frameNum=0; frameNum<st.FrameCount; frameNum++) {
				StackFrame frame=st.GetFrame(frameNum);
				Console.WriteLine(" at "+frame.GetMethod()+" in "+frame.GetFileName()+": line "+frame.GetFileLineNumber());
            }*/
		}
		
		[Conditional("DEBUG")]
		public static void StackTraceIf(bool condition) {
			if (condition) {
				StackTrace();
			}
		}
		
	}
}