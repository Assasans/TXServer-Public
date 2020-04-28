## Message structure.
Every message consists of 5 parts:
1) Signature;
2) OptionalMap length in bits;
3) Data length in bytes;
4) OptionalMap;
5) Actual data.

## Signature.
Contains { 0xFF, 0x00 } bytes.
This is only item that can be used in bound checking.

## OptionalMap.
Bitmask.
Bit necessarity defined as:
1) [ProtocolOptional] attribute on properties;
2) Optional<T> type;
3) OptionalTypeCodec type in code.
  (Server implements this list as [OptionalMapped] attribute.)
Should be read in left-to-right order.
Bit contains 1 when corresponding encoded item is null, 0 when non-null.

## Data
Every encoded data type MUST have appropriate type
  derived from Codec interface and be registered in code.
  (This statement currently applies only for client part)
For further information, look into Codec type and its derived types.

Data is split in commands.
Every command contains leading CommandCode enum.
  (Server implements it as [CommandCode(int)] attribute)
For further information, look into CommandCode enum & EnumCodec.

Any data contained within commands IS sorted in alphabetical order,
  if this behaviour is not overridden by [ProtocolParameterOrder(int)] attribute.
  (To achieve same result, server uses [ProtocolFixed] attribute.)


## ECS system.
Entity contains:
1) TemplateAccessor, which on its own contains:
  1) Template type;
  2) ConfigPath string.
2) Component list.

ConfigPath contains path relative to config root.
Directory referenced by ConfigPath MUST contain public.yml file and this file MUST contain
  all fields referenced by corresponding Template type fields.
ConfigPath may be left empty if Template type does not reference anything.

Events can be send to one or multiple entities.