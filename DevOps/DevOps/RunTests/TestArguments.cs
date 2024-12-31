//
// Rau - An AT-Proto Bot Framework
// Copyright (C) 2024 Seth Hendrick
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

using Cake.ArgumentBinder;

namespace DevOps.RunTests
{
    public sealed class TestArguments
    {
        // ---------------- Fields ----------------

        public static readonly string CoverageFilter = "+[*]Rau* -[Rau.Tests*]*";

        // ---------------- Constructor ----------------

        public TestArguments()
        {
            this.RunWithCodeCoverage = false;
        }

        // ---------------- Properties ----------------

        [BooleanArgument(
            "code_coverage",
            Description = "Should we run this with code coverage?",
            DefaultValue = false
        )]
        public bool RunWithCodeCoverage { get; set; }
    }
}
