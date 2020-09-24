using System;
using SlangLang.Symbols;

namespace SlangLang.Binding
{
    internal sealed class Conversion
    {
        public static readonly Conversion None = new Conversion(false, false, false);
        public static readonly Conversion Identity = new Conversion(true, true, true);
        public static readonly Conversion Implicit = new Conversion(true, false, true);
        public static readonly Conversion Explicit = new Conversion(true, false, false);
        
        public static Conversion Classify(TypeSymbol from, TypeSymbol to)
        {
            if (from == to)
            {
                return Conversion.Identity;
            }
            
            if (from == TypeSymbol.Int || from == TypeSymbol.Bool)
            {
                if (to == TypeSymbol.String)
                {
                    return Conversion.Explicit;
                }
            }

            return Conversion.None;
        }

        public readonly bool exists;
        public readonly bool isIdentity;
        public readonly bool isImplicit;
        public readonly bool isExplicit;

        private Conversion(bool exists, bool isIdentity, bool isImplicit)
        {
            this.exists = exists;
            this.isIdentity = isIdentity;
            this.isImplicit = isImplicit;
            this.isExplicit = exists && !isImplicit;
        }
    }
}