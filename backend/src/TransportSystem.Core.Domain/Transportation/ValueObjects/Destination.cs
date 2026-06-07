using TransportSystem.Core.Domain.Common;

namespace TransportSystem.Core.Domain.Transportation.ValueObjects
{
    public class Destination : ValueObject
    {
        public string Name { get; }

        private Destination(string name)
        {
            Name = name;
        }

        public static Destination Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("DESTINATION_EMPTY",
                    "El destino no puede estar vacío.");

            if (name.Trim().Length > 300)
                throw new DomainException("DESTINATION_TOO_LONG",
                    "El destino no puede superar 300 caracteres.");

            return new Destination(name.Trim());
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Name;
        }

        public override string ToString() => Name;
    }
}
