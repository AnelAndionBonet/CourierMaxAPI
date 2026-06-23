namespace CourierMax.UseCases;

public record class ShipmentsUseCases(
    CreateShipmentCommand CreateShipmentCommand,
    ChangeShipmentStateCommand ChangeShipmentStateCommand,
    AssignShipmentCommand AssignShipmentCommand,
    GetShipmentQuery GetShipmentQuery,
    GetDelayedShipmentsQuery GetDelayedShipmentsQuery);
