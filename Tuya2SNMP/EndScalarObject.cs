using Lextm.SharpSnmpLib;
using Lextm.SharpSnmpLib.Pipeline;

namespace Tuya2SNMP
{
    internal class EndScalarObject : ScalarObject
    {

        public EndScalarObject()
            : base(new ObjectIdentifier("1.3.9999"))
        { }

        public override ISnmpData Data
        {
            get => new Integer32(0);
            set { }
        }

    }
}