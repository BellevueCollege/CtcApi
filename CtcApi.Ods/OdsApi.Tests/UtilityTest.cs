//Copyright (C) 2011 Bellevue College and Peninsula College
//
//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as
//published by the Free Software Foundation, either version 3 of the
//License, or (at your option) any later version.
//
//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU Lesser General Public License for more details.
//
//You should have received a copy of the GNU Lesser General Public
//License and GNU General Public License along with this program.
//If not, see <http://www.gnu.org/licenses/>.
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ctc.Ods.Tests
{
    [TestClass]
    public class UtilityTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

    	[TestMethod]
    	public void SafeConvertToBool_TrueString()
    	{
    		bool actual = Utility.SafeConvertToBool("true");

    		Assert.IsTrue(actual);
    	}

    	[TestMethod]
    	public void SafeConvertToBool_NumericString()
    	{
    		bool actual = Utility.SafeConvertToBool("1");

    		Assert.IsFalse(actual);
    	}

    	[TestMethod]
    	public void SafeConvertToBool_ZeroString()
    	{
    		bool actual = Utility.SafeConvertToBool("0");

    		Assert.IsFalse(actual);
    	}

    	[TestMethod]
    	public void SafeConvertToBool_NegativeNumberString()
    	{
    		bool actual = Utility.SafeConvertToBool("-1");

    		Assert.IsFalse(actual);
    	}

    	[TestMethod]
    	public void SafeConvertToBool_EmptyString()
    	{
    		bool actual = Utility.SafeConvertToBool("");

    		Assert.IsFalse(actual);
    	}

    	[TestMethod]
    	public void SafeConvertToBool_StringEmpty()
    	{
    		bool actual = Utility.SafeConvertToBool(string.Empty);

    		Assert.IsFalse(actual);
    	}

    }
}
