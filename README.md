# TXServer

# Building and running

- `dotnet run --project TXServerConsole --run <address> <port> <max players>`
- 10 maximum players: `dotnet run --project TXServerConsole --run 0.0.0.0 5050 10`
- Loopback, 5 maximum players: `dotnet run --project TXServerConsole --run 127.0.0.1 5050 5`

# Protocol

## Message structure

Every message consists of 5 parts:

- Signature
- `OptionalMap` length in bits
- Data length in bytes
- `OptionalMap`
- Actual data

## Signature

Contains `{ 0xFF, 0x00 }` bytes. This is the only thing that can be used in bound checking.

## OptionalMap

`OptionalMap` **must** be read in left-to-right order.  
Bit is **set** when corresponding encoded item is `null`, **not set** when it is not `null`.

Property nullability defined with:

- `[ProtocolOptional]` attribute on properties
- `Optional<T>` type
- `OptionalTypeCodec` type in code

Server supports nullable properties with `[OptionalMapped]` attribute.  

## Data

Every encoded data type **must** have appropriate type derived from `Codec` interface and be registered in code *(this statement currently applies only for client part)*.  
For further information, look into `Codec` type and its derived types.  

Data is split in commands.  
Every command contains leading `CommandCode` enum *(server implements it as `[CommandCode(int)]` attribute)*.  
For further information, look into `CommandCode` enum & `EnumCodec`.  

Any data contained within commands **must** be sorted in alphabetical order, this behaviour can be overridden by `[ProtocolParameterOrder(int position)]` attribute *(server uses `[ProtocolFixed(int position)]` attribute)*.

## Entity-Component-System

Entity contains:

- `TemplateAccessor` which contains:
  - `Template` type
  - `ConfigPath` string
- `Component` list

`ConfigPath` contains path relative to config root.  
Directory referenced by `ConfigPath` **must** contain `public.yml` file and this file **must** contain all properties referenced by corresponding `Template` type fields.  
`ConfigPath` may be left empty if `Template` type does not reference anything.  

Events can be sent to one or multiple entities.
