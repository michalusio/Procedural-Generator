namespace Packages.pl.lochalhost.procedural_generator.Editor.Packages.pl.lochalhost.procedural_generator.Editor.Base
{
    internal interface ISerializable<T, V> where T: ISerializable<T, V>
    {
        V Serialize();
    }
}
