//
// Rau - An AT-Proto Bot Framework
// Copyright (C) 2024-2025 Seth Hendrick
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rau.Tests
{
    public static class AssertEx
    {
        public static void ArePdsPostsEqual( PdsPost expected, PdsPost actual )
        {
            try
            {
                if( expected.Languages is null )
                {
                    Assert.IsNull( actual.Languages );
                }
                else if( actual.Languages is null )
                {
                    Assert.IsNull( expected.Languages );
                }
                else
                {
                    Assert.IsTrue( expected.Languages.SequenceEqual( actual.Languages ) );
                }
                Assert.AreEqual( expected.PostContents, actual.PostContents );
                Assert.AreEqual( expected.PostAttachmentPage, actual.PostAttachmentPage );

                if( expected.PostImages is null )
                {
                    Assert.IsNull( actual.PostImages );
                }
                else if( actual.PostImages is null )
                {
                    Assert.IsNull( expected.PostImages );
                }
                else
                {
                    Assert.IsTrue( expected.PostImages.SequenceEqual( actual.PostImages ) );
                }
            }
            catch( Exception )
            {
                Console.WriteLine( "Expected: " + expected );
                Console.WriteLine( "Actual: " + actual );
                throw;
            }
        }
    }
}
