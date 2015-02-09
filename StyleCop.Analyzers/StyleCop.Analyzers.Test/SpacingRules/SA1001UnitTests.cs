﻿namespace StyleCop.Analyzers.Test.SpacingRules
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CodeFixes;
    using Microsoft.CodeAnalysis.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using StyleCop.Analyzers.SpacingRules;
    using TestHelper;

    /// <summary>
    /// This class contains unit tests for <see cref="SA1001CommasMustBeSpacedCorrectly"/> and
    /// <see cref="SA1001CodeFixProvider"/>.
    /// </summary>
    [TestClass]
    public class SA1001UnitTests : CodeFixVerifier
    {
        private string DiagnosticId { get; } = SA1001CommasMustBeSpacedCorrectly.DiagnosticId;

        protected override CodeFixProvider GetCSharpCodeFixProvider()
        {
            return new SA1001CodeFixProvider();
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new SA1001CommasMustBeSpacedCorrectly();
        }

        [TestMethod]
        public async Task TestNoSpaceAfterComma()
        {
            string statementWithoutSpace = @"f(a,b);";
            string statementWithSpace = @"f(a, b);";

            await TestCommaInStatementOrDecl(statementWithSpace, EmptyDiagnosticResults, statementWithSpace);

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Commas must be followed by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 16)
                            }
                    }
                };


            await TestCommaInStatementOrDecl(statementWithoutSpace, expected, statementWithSpace);
        }

        [TestMethod]
        public async Task TestSpaceBeforeComma()
        {
            string spaceBeforeComma = @"f(a , b);";
            string spaceOnlyAfterComma = @"f(a, b);";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Commas must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 17)
                            }
                    },
                };


            await TestCommaInStatementOrDecl(spaceBeforeComma, expected, spaceOnlyAfterComma);
        }

        [TestMethod]
        public async Task TestSpaceBeforeCommaAtEndOfLine()
        {
            string spaceBeforeComma = $"f(a ,{Environment.NewLine}b);";
            string spaceOnlyAfterComma = $"f(a,{Environment.NewLine}b);";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Commas must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 17)
                            }
                    },
                };

            await TestCommaInStatementOrDecl(spaceBeforeComma, expected, spaceOnlyAfterComma);
        }

        [TestMethod]
        public async Task TestLastCommaInLine()
        {
            string statement = @"f(a,
              b);";
            await TestCommaInStatementOrDecl(statement, EmptyDiagnosticResults, statement);
        }

        [TestMethod]
        public async Task TestFirstCommaInLine()
        {
            string statement = $"f(a{Environment.NewLine}, b);";
            await TestCommaInStatementOrDecl(statement, EmptyDiagnosticResults, statement);
        }

        [TestMethod]
        public async Task TestCommentBeforeFirstCommaInLine()
        {
            string statement = $"f(a // comment{Environment.NewLine}, b);";
            await TestCommaInStatementOrDecl(statement, EmptyDiagnosticResults, statement);
        }

        [TestMethod]
        public async Task TestSpaceBeforeCommaFollowedByAngleBracketInFuncType()
        {
            string statement = @"var a = typeof(System.Func< ,>);";
            string fixedStatement = @"var a = typeof(System.Func<,>);";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Commas must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 41)
                            }
                    },
                };

            await TestCommaInStatementOrDecl(statement, expected, fixedStatement);
        }

        [TestMethod]
        public async Task TestCommaFollowedByAngleBracketInFuncType()
        {
            string statement = @"var a = typeof(System.Func<,>);";
            await TestCommaInStatementOrDecl(statement, EmptyDiagnosticResults, statement);
        }

        [TestMethod]
        public async Task TestSpaceBeforeCommaFollowedByCommaInFuncType()
        {
            string statement = @"var a = typeof(System.Func< ,,>);";
            string fixedStatement = @"var a = typeof(System.Func<,,>);";

            DiagnosticResult[] expected;

            expected =
                new[]
                {
                    new DiagnosticResult
                    {
                        Id = DiagnosticId,
                        Message = "Commas must not be preceded by a space.",
                        Severity = DiagnosticSeverity.Warning,
                        Locations =
                            new[]
                            {
                                new DiagnosticResultLocation("Test0.cs", 7, 41)
                            }
                    },
                };

            await TestCommaInStatementOrDecl(statement, expected, fixedStatement);
        }

        [TestMethod]
        public async Task TestCommaFollowedByCommaInFuncType()
        {
            string statement = @"var a = typeof(System.Func<,,>);";
            await TestCommaInStatementOrDecl(statement, EmptyDiagnosticResults, statement);
        }

        private async Task TestCommaInStatementOrDecl(string originalStatement, DiagnosticResult[] expected, string fixedStatement)
        {
            string template = @"namespace Foo
{{
    class Bar
    {{
        void Baz()
        {{
            {0}
        }}
    }}
}}
";
            string originalCode = string.Format(template, originalStatement);
            string fixedCode = string.Format(template, fixedStatement);

            await VerifyCSharpDiagnosticAsync(originalCode, expected, CancellationToken.None);
            await VerifyCSharpFixAsync(originalCode, fixedCode, cancellationToken: CancellationToken.None);
        }
    }
}
