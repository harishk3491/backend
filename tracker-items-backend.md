# Item Microservice — Development Tracker

## Project Info
- Tech: ASP.NET Core 10 Web API, EF Core Code First, Clean Architecture
- DB: Server=LAPTOP-TH3F0HAS;Database=ErpItemServiceDb;Trusted_Connection=True;TrustServerCertificate=True;
- Architecture: Simple CRUD microservice (demo)
- Source SRS: D:\Uniphore\knowledge-base\wiki\SRS\Items\
- Coding Guidelines: D:\Uniphore\knowledge-base\wiki\Code\Backend\development-standards.md (partial)
- Schema: D:\Uniphore\knowledge-base\CLAUDE.md
- Graph: D:\Uniphore\knowledge-base\graphify-out\
- Skipped: Auth, Testing, CI/CD

## Solution Structure

```
Items.slnx
└── src/
    ├── Items.Domain/          ← entities, exceptions (no dependencies)
    ├── Items.Application/     ← DTOs, service interfaces, DI
    ├── Items.Infrastructure/  ← EF Core DbContext, migrations, DI
    └── Items.API/             ← controllers, middleware, Program.cs
```

---

## Phases

### Phase 0: Scaffolding [DONE - 2026-04-14]
- [x] Create solution & project structure (Clean Architecture, 4 projects)
- [x] Configure EF Core with SQL Server (Items.Infrastructure DI)
- [x] Verify DB connection (ef database update → Done)
- [x] Base entity class (Domain/Common/BaseEntity.cs)
- [x] DbContext with audit-field hook (Infrastructure/Persistence/ItemsDbContext.cs)
- [x] Global exception middleware → ProblemDetails responses
- [x] Serilog structured logging wired in Program.cs
- [x] Initial EF migration (empty, creates __EFMigrationsHistory)

---

### Phase 1: Item Settings [DONE - 2026-04-14]

**SRS source:** wiki/SRS/Items/item-settings.md
**Entity:** `ItemSettings` — singleton global config (one row per tenant/org)

Fields to persist:
- `ItemCodeMaxLength` (int, 1–50)
- `AllowItemLevelCustomization` (bool)
- `LogItemClassOverrides` (bool)
- `AllowDuplicateNames` (bool)
- `AllowTagNumberInSalesDocs` (bool)
- `EnableAutoBatchSerialGeneration` (bool)
- `AutoGenPrefix`, `AutoGenSuffix`, `AutoGenSeparator` (string?)
- `AutoGenSequenceStart`, `AutoGenSequenceIncrement` (int?)
- `CopyDescription`, `CopyTechnicalSpecs`, `CopyPricingDetails` (bool)
- `CopyTaxDetails`, `CopyItemClassDefaults`, `CopyAttachments` (bool)
- `EcnEntryPermission` (bool)
- `CcnNumberingBasis` (enum: ParentItem | SalesJobOrder)

Tasks:
- [x] `ItemSettings` entity in Domain
- [x] Migration: `AddItemSettings`
- [x] `GET /api/v1/item-settings` — read singleton
- [x] `PUT /api/v1/item-settings` — upsert singleton

---

### Phase 2: Unit of Measure (UOM) [DONE - 2026-04-14]

**SRS source:** Referenced throughout item-master.md, item-vendor-mapping.md
**Entity:** `UnitOfMeasure` — simple lookup master

Fields:
- `Code` (string, unique, max 20)
- `Name` (string, max 100)
- `Description` (string?, max 500)

Tasks:
- [x] `UnitOfMeasure` entity in Domain
- [x] Migration: `AddUnitOfMeasure`
- [x] `GET    /api/v1/uoms`
- [x] `GET    /api/v1/uoms/{id}`
- [x] `POST   /api/v1/uoms`
- [x] `PUT    /api/v1/uoms/{id}`
- [x] `DELETE /api/v1/uoms/{id}` (soft-delete; block if used in any item)

---

### Phase 3: Item Class [DONE - 2026-04-14]

**SRS source:** wiki/SRS/Items/item-class.md
**Entity:** `ItemClass` — behavioral template inherited by items

Key fields (8 sections from SRS):
- **Identity:** `Code` (system-gen, unique), `Name` (max 100), `Description`
- **Nature & Flow:** `ItemNature` (enum: Goods | Service), `ItemType` (enum: RM|BO|RS|MK|AS|SP|TL|CP|CO|SR), `Purchasable`, `Saleable`
- **Inventory & Valuation:** `InventoryManaged`, `ValuationMethod` (FIFO|MovingAverage|Standard), `AllowExcessGrnPercent`, `IssueMethod` (Manual|Backflush), `WoReceiptCostingViaExtraIssue`
- **Traceability:** `TraceabilityLevel` (None|Batch|Serial), `InwardNumberTracking`, `HeatNumberTracking`, `ShelfLifeApplicable`, `AllowIssueIfExpired`, `IgnoreDayInExpiry`, `CannotSellExpiredInNextDays`
- **QC:** `QcRequired`, `TpiRequired`, `TpiTriggerStage`, `TpiCertificateMandatory`, `TpiCaptureRemarks`, `BlockDispatchUntilTpiAccepted`, `AllowTpiOverrideAtSo`
- **Planning/MRP:** `IndentMandatory`, `FastMovingThreshold`, `SlowMovingThreshold`, `OrderReleadLeadTime`, `DeliveryReleaseLeadTime`, `MinimumProductionLogic`, `MinimumProductionQty`, `ProductionBatchMultiple`, `AutoAllocation`, `AutoAllocationOn`
- **Dimensions:** `DimensionBasedItem`, `NeedDimensionWiseStockKeeping`, `NeedDimensionWiseConsumptionInBom`, `LongItem`, `BomQtyInDimensionsAndPieces`
- **Finance:** `SalesAccountGroup`, `SalesAccount`, `PurchaseAccountGroup`, `PurchaseAccount`

Tasks:
- [x] `ItemClass` entity + `ItemNature`/`ItemType`/`ValuationMethod`/`TraceabilityLevel`/`IssueMethod`/`TpiTriggerStage`/`AutoAllocationOn` enums
- [x] Migration: `AddItemClass`
- [x] `GET    /api/v1/item-classes`       (paginated)
- [x] `GET    /api/v1/item-classes/{id}`
- [x] `POST   /api/v1/item-classes`
- [x] `PUT    /api/v1/item-classes/{id}`
- [x] `DELETE /api/v1/item-classes/{id}` (soft-delete)
- [x] `POST   /api/v1/item-classes/{id}/clone`

---

### Phase 4: Item Master [DONE - 2026-04-14]

**SRS source:** wiki/SRS/Items/item-master.md
**Entities:** `Item`, `ItemAlternateUom`, `ItemTechnicalSpec`, `ItemDrawing`, `ItemWarehouseThreshold`

Key fields (7 sections from SRS):
- **Identity:** `Code` (system-gen, unique), `Name` (max 100), `Description`, `Sku` (unique org-wide), `ParentItemId?`, `VariantName?`, `VariantCode?`
- **Classification:** `ItemClassId?`, `ItemType`*, `ItemNature`*, `ReferenceCode?`, `Manufacturer?`, `KanbanItem`, `MmfgItem`, `MinMassManufacturingQty?`, `WantQrCode`
- **Units & Physical:** `BaseUomId`, `MaterialOfConstruction`, `StandardWeight?`
- **Dimensions:** (inherited from ItemClass flags)
- **Tax & Regulatory:** `HsnSacCode`, `TaxPreference` (Taxable|NonTaxable), `TaxRate?`, `CountryOfOrigin`
- **Commercial:** `PurchaseCost?`, `SalesPrice?`, `SubContractingCost?`, `DiscountEligible`
- **Tool Details:** `IsToolConsumable?`, `ConsumptionType?`, `MaintenanceRequired?`, `CalibrationRequired?`, etc.

Sub-entities:
- `ItemAlternateUom`: `ItemId`, `UomId`, `ConversionFactor`, `IsPerfect`
- `ItemTechnicalSpec`: `ItemId`, `CapabilityType`, `CapabilityValue`
- `ItemDrawing`: `ItemId`, `DrawingName`, `DrawingNumber`, `Revision`, `Size`, `ShowInProductionBooking`
- `ItemWarehouseThreshold`: `ItemId`, `WarehouseName`, `MinThreshold`, `MaxThreshold`, `ReorderQty`, `OpeningQty?`, `OpeningRate?`, `OpeningDate?`

Tasks:
- [x] All entities above
- [x] Migration: `AddItemMaster`
- [x] `GET    /api/v1/items`            (paginated, filterable by class/type/nature)
- [x] `GET    /api/v1/items/{id}`
- [x] `POST   /api/v1/items`
- [x] `PUT    /api/v1/items/{id}`
- [x] `DELETE /api/v1/items/{id}`       (soft-delete; block if transaction history)
- [x] `POST   /api/v1/items/{id}/clone`

---

### Phase 5: Item Vendor Mapping [DONE - 2026-04-14]

**SRS source:** wiki/SRS/Items/item-vendor-mapping.md
**Entities:** `ItemVendorMapping`, `ItemVendorPurchaseUom`, `ItemVendorPricing`

Key fields:
- **Core:** `Code` (system-gen), `ItemId`, `VendorId` (int; Vendor master is external), `VendorItemCode?`, `VendorSku?`, `IsPreferredVendor`
- **Purchase UOM grid:** `PurchaseUomId`, `IsPerfectConversion`, `ConversionRate`, `TolerancePercent`
- **Logistics:** `Moq`, `MaxOrderQty?`, `Eoq?`, `StandardLeadTimeDays`, `WarrantyDuration?`, `WarrantyDurationUnit?`, `WarrantyStartBasis?`
- **Pricing:** `BasePurchasePrice`, `Currency`, `DiscountType?`, `DiscountValue?`, `EffectivePurchasePrice` (computed), `PriceValidFrom?`, `PriceValidTo?`, `PaymentTerms`

Tasks:
- [x] All entities above
- [x] Migration: `AddItemVendorMapping`
- [x] `GET    /api/v1/item-vendor-mappings`      (filterable by itemId, vendorId)
- [x] `GET    /api/v1/item-vendor-mappings/{id}`
- [x] `POST   /api/v1/item-vendor-mappings`
- [x] `PUT    /api/v1/item-vendor-mappings/{id}`
- [x] `DELETE /api/v1/item-vendor-mappings/{id}` (soft-delete; block if active PO)
- [x] `POST   /api/v1/item-vendor-mappings/{id}/clone`

---

### Phase 6: Price Lists [DONE - 2026-04-14]

**SRS source:** wiki/SRS/Items/price-lists.md
**Entities:** `PriceList`, `PriceListItem`

Key fields:
- **Header:** `Code` (system-gen), `ApplicableTo` (enum: Sales|Purchase), `Name` (max 100), `Description?`, `Status` (Draft|Active|Inactive|Expired), `Priority` (int, higher = higher priority), `Currency`
- **Applicability:** `TargetType` (AllCustomers|SpecificCustomers|AllVendors|SpecificVendors|Region), `TargetIds` (JSON or separate join table)
- **Pricing scope:** `PricingScopeType` (AllItems|SelectedItems|SelectedItemClasses)
- **PriceListItem sub-grid:** `ItemId?`, `ItemClassId?`, `MinQty?`, `MaxQty?`, `BasePrice`, `DiscountType?`, `DiscountValue?`, `FinalUnitPrice` (computed), `RoundOffTo?`, `ValidFrom?`, `ValidTo?`

Tasks:
- [x] All entities above
- [x] Migration: `AddPriceLists`
- [x] `GET    /api/v1/price-lists`
- [x] `GET    /api/v1/price-lists/{id}`
- [x] `POST   /api/v1/price-lists`
- [x] `PUT    /api/v1/price-lists/{id}`
- [x] `DELETE /api/v1/price-lists/{id}` (soft-delete; block if linked to active SO/contract)
- [x] `POST   /api/v1/price-lists/{id}/clone`

---

---

### Phase 7: FluentValidation [DONE - 2026-04-14]

**Purpose:** Add server-side DTO validation using FluentValidation for all request types across all six entities. Validators live in `Items.Application` alongside their DTOs. Registered via `AddValidatorsFromAssembly` + `AddFluentValidationAutoValidation` in the Application DI. Validation errors surface as `400 Bad Request` with `ProblemDetails` (already handled by global middleware).

**Dependencies:** `FluentValidation.AspNetCore` NuGet package (Items.Application + Items.API).

**Ambiguities to resolve before coding:**
1. Max lengths for `AutoGenPrefix` / `AutoGenSuffix` / `AutoGenSeparator` — default to 50 unless SRS updated
2. `HsnSacCode` external API validation — defer to service layer; validator checks non-empty only
3. `MaterialOfConstruction` — treat as free-text string for now; required non-empty
4. `AllowExcessGrnPercent` upper bound — enforce 0–100 per SRS literal
5. **PriceList Priority contradiction** — SRS grid says "lower = higher priority" but Create screen says "higher = higher"; our implementation uses higher = higher; add a comment in validator
6. Slab non-overlapping & alternate-UOM duplicate checks — service-layer only (too stateful for FluentValidation)

**Checklist:**

- [x] Add FluentValidation packages to Items.Application and Items.API
- [x] Register validators in Application DI (`AddValidatorsFromAssembly`)
- [x] Wire `AddFluentValidationAutoValidation` in Items.API `Program.cs`
- [x] `UpdateItemSettingsRequestValidator` — `ItemCodeMaxLength` 1–50; `AutoGenSequenceIncrement` > 0 if set; enum validity for `CcnNumberingBasis`
- [x] `CreateUomRequestValidator` / `UpdateUomRequestValidator` — `Code` required max 20; `Name` required max 100; `Description` max 500
- [x] `CreateItemClassRequestValidator` — all Section 1–8 rules; conditional rules (ItemNature=Service disables inventory; at-least-one Purchasable/Saleable; FastMoving > SlowMoving; conditional sub-fields for TPI, Shelf Life, MinProd, AutoAlloc, Dimensions)
- [x] `UpdateItemClassRequestValidator` — same rules minus `ItemNature` (non-editable post-create; reject if changed)
- [x] `AlternateUomRequestValidator` (shared) — `UomId` required; `ConversionFactor` > 0
- [x] `TechnicalSpecRequestValidator` (shared) — `CapabilityType` ↔ `CapabilityValue` mutual-required
- [x] `DrawingRequestValidator` (shared) — `DrawingName` max 100
- [x] `WarehouseThresholdRequestValidator` (shared) — `WarehouseName` required; `MinThreshold` > 0; `MaxThreshold` > Min; `ReorderQty` > 0; `OpeningRate` > 0 if `OpeningQty > 0`; `OpeningDate` not future if provided
- [x] `CreateItemRequestValidator` — Section 1–8 rules; `Name` max 100; `Sku` required; `VariantName/Code` required if `ParentItemId` set; tool-detail conditionals; dimension conditionals; tax conditionals; commercial price conditionals; applies child validators via `RuleForEach`
- [x] `UpdateItemRequestValidator` — same rules; `ItemNature` / dimension flags non-editable (reject if changed)
- [x] `PurchaseUomRequestValidator` (shared) — `PurchaseUomId` required; `ConversionRate` > 0; `TolerancePercent` >= 0
- [x] `VendorPricingRequestValidator` (shared) — `BasePurchasePrice` > 0; `Currency` required; `DiscountValue` 0–100 if Percent; `PriceValidTo` > `PriceValidFrom` if both set; `PaymentTerms` required
- [x] `CreateItemVendorMappingRequestValidator` — `ItemId` + `VendorId` required; child validators for UOM and pricing rows
- [x] `UpdateItemVendorMappingRequestValidator` — Moq/LeadTime; child validators (ItemId/VendorId non-editable post-create per SRS)
- [x] `PriceListItemRequestValidator` (shared) — `ItemId` or `ItemClassId` at least one; `BasePrice` > 0; `DiscountValue` < 100 if Percent; `MaxQty` >= `MinQty` if both set; `ValidTo` > `ValidFrom` if both set
- [x] `CreatePriceListRequestValidator` — `ApplicableTo` required; `Name` required max 100; `Priority` > 0; `Currency` required; `Status` valid enum; child validator for line items
- [x] `UpdatePriceListRequestValidator` — same rules

---

---

### Phase 8: AutoMapper — Centralized Mapping Profiles [DONE - 2026-04-14]

**Purpose:** Eliminate ~40+ manual `MapToDto()` methods and inline `new Entity { ... }` object initializers scattered across 6 service files by introducing AutoMapper with one profile per module. All profiles live in `Items.Application` alongside their DTOs (Clean Architecture — Application owns mapping).

**Dependencies:** `AutoMapper` + `AutoMapper.Extensions.Microsoft.DependencyInjection` NuGet packages in `Items.Application`.

**Architecture Decision — One profile per module (not one global profile):**
Clean Architecture favors cohesion within vertical slices. Each module's mapping concerns are self-contained. A single `MappingProfile.cs` grows unmanageable once you have 35+ mapping pairs. Six focused profiles (`ItemClassMappingProfile`, `ItemMappingProfile`, etc.) are easier to locate, test, and reason about. All are discovered automatically via `AddAutoMapper(Assembly.GetExecutingAssembly())`.

**What AutoMapper will and will NOT replace:**
- ✅ `MapToDto()` private methods in all 6 services → `_mapper.Map<TDto>(entity)`
- ✅ LINQ `Select(i => new SummaryDto { ... })` in `GetAllAsync` → `ProjectTo<TSummaryDto>(_mapper.ConfigurationProvider)` (pushes projection to SQL)
- ✅ `new Entity { Prop = req.Prop, ... }` in `CreateAsync` → `_mapper.Map<TEntity>(request)`
- ✅ Line-by-line property assignment in `UpdateAsync` / `ApplyRequest` → `_mapper.Map(request, existingEntity)`
- ❌ Clone operations — business logic (code generation, `Status = Draft`, deep copy of collections) is NOT pure mapping; leave manual
- ❌ Computed fields (`EffectivePurchasePrice`, `FinalUnitPrice`) — computed at service layer before persist; ignore in Request→Entity maps, include as regular property in Entity→Dto maps

**Special ForMember cases:**
- `Item → ItemSummaryDto`: `ItemClassName` from `item.ItemClass.Name` (nullable nav); `BaseUomCode` from `item.BaseUom.Code`
- `ItemAlternateUom → ItemAlternateUomDto`: `UomCode` from `Uom.Code` (nav property)
- `UpdateItemClassRequest → ItemClass`: ignore `ItemNature` (non-editable post-create per SRS)
- `UpdateItemRequest → Item`: ignore `ItemNature`, dimension flag fields (non-editable per SRS)
- All Request → Entity maps: ignore `Id`, `Code` (system-generated), `CreatedAt`, `UpdatedAt`, `IsActive`
- All `CreateXyzRequest → XyzSubEntity` maps: ignore the parent FK (`ItemId`, `ItemVendorMappingId`, etc.) — service sets these after creation

**Complete mapping pairs (35 pairs across 6 profiles):**

| Profile | Source | Destination | Notes |
|---------|--------|-------------|-------|
| `ItemSettingsMappingProfile` | `ItemSettings` | `ItemSettingsDto` | Flat 1:1 |
| | `UpdateItemSettingsRequest` | `ItemSettings` | `Map(src, dest)` overload (apply to existing) |
| `UomMappingProfile` | `UnitOfMeasure` | `UomDto` | Flat 1:1 |
| | `CreateUomRequest` | `UnitOfMeasure` | Ignore `Id`, `Code`, audit fields |
| | `UpdateUomRequest` | `UnitOfMeasure` | `Map(src, dest)` overload |
| `ItemClassMappingProfile` | `ItemClass` | `ItemClassSummaryDto` | 6 properties, flat |
| | `ItemClass` | `ItemClassDto` | ~75 properties, flat |
| | `CreateItemClassRequest` | `ItemClass` | Ignore `Id`, `Code`, audit fields |
| | `UpdateItemClassRequest` | `ItemClass` | `Map(src, dest)`; `Ignore(ItemNature)` |
| `ItemMappingProfile` | `Item` | `ItemSummaryDto` | `ForMember(ItemClassName, o => o.MapFrom(i => i.ItemClass != null ? i.ItemClass.Name : null))`; `ForMember(BaseUomCode, o => o.MapFrom(i => i.BaseUom.Code))` |
| | `Item` | `ItemDto` | ~120 properties; nested collections auto-resolved |
| | `ItemAlternateUom` | `ItemAlternateUomDto` | `ForMember(UomCode, o => o.MapFrom(a => a.Uom.Code))` |
| | `CreateItemAlternateUomRequest` | `ItemAlternateUom` | Ignore `Id`, `ItemId`, `Uom` nav |
| | `ItemTechnicalSpec` | `ItemTechnicalSpecDto` | Flat |
| | `CreateItemTechnicalSpecRequest` | `ItemTechnicalSpec` | Ignore `Id`, `ItemId` |
| | `ItemDrawing` | `ItemDrawingDto` | Flat |
| | `CreateItemDrawingRequest` | `ItemDrawing` | Ignore `Id`, `ItemId` |
| | `ItemWarehouseThreshold` | `ItemWarehouseThresholdDto` | Flat |
| | `CreateItemWarehouseThresholdRequest` | `ItemWarehouseThreshold` | Ignore `Id`, `ItemId` |
| | `CreateItemRequest` | `Item` | Ignore `Id`, `Code`, audit, sub-collection navs (service maps separately) |
| | `UpdateItemRequest` | `Item` | `Map(src, dest)`; ignore `ItemNature`, dimension flags |
| `VendorMappingMappingProfile` | `ItemVendorMapping` | `ItemVendorMappingSummaryDto` | Flat; ForMember for any nav-derived fields |
| | `ItemVendorMapping` | `ItemVendorMappingDto` | Nested `PurchaseUoms` + `Pricing` collections |
| | `ItemVendorPurchaseUom` | `ItemVendorPurchaseUomDto` | Flat |
| | `CreateItemVendorPurchaseUomRequest` | `ItemVendorPurchaseUom` | Ignore `Id`, `ItemVendorMappingId` |
| | `ItemVendorPricing` | `ItemVendorPricingDto` | Flat |
| | `CreateItemVendorPricingRequest` | `ItemVendorPricing` | Ignore `Id`, `ItemVendorMappingId`, `EffectivePurchasePrice` |
| | `CreateItemVendorMappingRequest` | `ItemVendorMapping` | Ignore `Id`, `Code`, audit, collection navs |
| | `UpdateItemVendorMappingRequest` | `ItemVendorMapping` | `Map(src, dest)`; ignore `ItemId`, `VendorId` |
| `PriceListMappingProfile` | `PriceList` | `PriceListSummaryDto` | Flat |
| | `PriceList` | `PriceListDto` | Nested `Items` collection |
| | `PriceListItem` | `PriceListItemDto` | Flat |
| | `CreatePriceListItemRequest` | `PriceListItem` | Ignore `Id`, `PriceListId`, `FinalUnitPrice` |
| | `CreatePriceListRequest` | `PriceList` | Ignore `Id`, `Code`, audit, `Items` nav |
| | `UpdatePriceListRequest` | `PriceList` | `Map(src, dest)` |

**Profile file locations:**
```
Items.Application/
├── ItemSettings/Mappings/ItemSettingsMappingProfile.cs
├── Uoms/Mappings/UomMappingProfile.cs
├── ItemClasses/Mappings/ItemClassMappingProfile.cs
├── Items/Mappings/ItemMappingProfile.cs
├── ItemVendorMappings/Mappings/VendorMappingMappingProfile.cs
└── PriceLists/Mappings/PriceListMappingProfile.cs
```

**Checklist:**

- [x] Add `AutoMapper` 16.1.1 to `Items.Application.csproj`
- [x] Register `AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()))` in Application DI
- [x] Inject `IMapper` into all 6 service constructors
- [x] **ItemSettingsMappingProfile** — 2 maps; wired `MapToDto()` + `ApplyRequest()` → `_mapper.Map`
- [x] **UomMappingProfile** — 3 maps; `ProjectTo` in `GetAllAsync`; wired Create, Update
- [x] **ItemClassMappingProfile** — 4 maps; `ProjectTo` in `GetAllAsync`; wired Create, Update; ignore `ItemNature` on Update
- [x] **ItemMappingProfile** — 11 maps; ForMember for `ItemSummaryDto`/`ItemDto`; nested sub-entity maps; `ProjectTo` in `GetAllAsync`; wired all operations
- [x] **VendorMappingMappingProfile** — 8 maps; nested collection maps; `ProjectTo` in `GetAllAsync`; ignore ItemId/VendorId on Update
- [x] **PriceListMappingProfile** — 6 maps; LineItems sorted by MinQty in Entity→Dto; ignore `FinalUnitPrice` on request maps; `ProjectTo` in `GetAllAsync`
- [x] Removed all `MapToDto()` and `ApplyRequest()` private methods from services
- [x] `dotnet build Items.slnx` → 0 errors, 0 warnings

---

### Phase 9: Swagger — API Documentation [DONE - 2026-04-14]

**Purpose:** Add interactive API documentation via Swashbuckle so consumers (and the team) can explore and test all 28 endpoints from a browser. Enriches the spec with XML summaries, response envelopes, and logical grouping.

**Recommendation — Swashbuckle.AspNetCore (not NSwag):**
| | Swashbuckle | NSwag |
|---|---|---|
| ASP.NET Core integration | Native, first-class | Good but more config |
| XML doc support | Built-in | Built-in |
| Client code generation | Not built-in | Built-in |
| Setup complexity | Low | Medium |
| Community / samples | Very large | Smaller |

This project has no client-code-generation requirement (microservice demo). Swashbuckle is the standard choice and requires the least friction.

**Controller annotation gaps (current state):**
All 6 controllers already have `[ProducesResponseType]` on every action. What is missing:
1. No XML `<summary>` / `<param>` / `<returns>` doc comments on any action
2. No `[Produces("application/json")]` at class level (Swagger infers, but explicit is better)
3. No `[Tags("...")]` attribute — Swagger groups by controller name; explicit tags give cleaner names
4. `500 Internal Server Error` not documented on any endpoint (global middleware always returns 500 on unhandled exceptions)
5. `ItemSettingsController.GET` missing `[ProducesResponseType(404)]` — the singleton can theoretically return a default but this is fine as-is
6. Enum values appear as integers in Swagger by default — need `UseInlineDefinitionsForEnums` or `JsonStringEnumConverter`

**Tagging strategy:**
| Controller | Proposed Tag | Description |
|------------|--------------|-------------|
| `ItemSettingsController` | `Item Settings` | Global configuration singleton |
| `UomsController` | `Units of Measure` | UOM lookup master |
| `ItemClassesController` | `Item Classes` | Behavioral templates for items |
| `ItemsController` | `Items` | Item master with sub-entities |
| `ItemVendorMappingsController` | `Item Vendor Mappings` | Vendor-item purchase relationships |
| `PriceListsController` | `Price Lists` | Sales/purchase price list headers and slabs |

**Endpoints needing XML comments (all 28):**

| Controller | Action | Summary to add |
|------------|--------|----------------|
| ItemSettings | GET | Returns the singleton item settings record (defaults if never saved) |
| ItemSettings | PUT | Creates or updates the singleton item settings |
| Uoms | GET | Returns all active UOMs |
| Uoms | GET /{id} | Returns a single UOM by ID |
| Uoms | POST | Creates a new UOM |
| Uoms | PUT /{id} | Updates an existing UOM |
| Uoms | DELETE /{id} | Soft-deletes a UOM (blocked if used on any item) |
| ItemClasses | GET | Returns paginated list of active item classes |
| ItemClasses | GET /{id} | Returns a single item class by ID |
| ItemClasses | POST | Creates a new item class |
| ItemClasses | PUT /{id} | Updates an existing item class (ItemNature non-editable) |
| ItemClasses | DELETE /{id} | Soft-deletes an item class |
| ItemClasses | POST /{id}/clone | Clones an item class with a new code |
| Items | GET | Returns paginated list of items; filterable by classId / type / nature |
| Items | GET /{id} | Returns full item record including sub-entities |
| Items | POST | Creates a new item with all sub-entities |
| Items | PUT /{id} | Updates an item (ItemNature and dimension flags non-editable) |
| Items | DELETE /{id} | Soft-deletes an item (blocked if transaction history exists) |
| Items | POST /{id}/clone | Deep-clones an item including all sub-entities |
| ItemVendorMappings | GET | Returns paginated list; filterable by itemId / vendorId |
| ItemVendorMappings | GET /{id} | Returns full vendor mapping with UOM and pricing grids |
| ItemVendorMappings | POST | Creates a vendor-item mapping |
| ItemVendorMappings | PUT /{id} | Updates a vendor mapping (ItemId / VendorId non-editable) |
| ItemVendorMappings | DELETE /{id} | Soft-deletes (blocked if active PO exists) |
| ItemVendorMappings | POST /{id}/clone | Clones a vendor mapping |
| PriceLists | GET | Returns paginated list of price lists |
| PriceLists | GET /{id} | Returns a price list with all line items |
| PriceLists | POST | Creates a price list with line items |
| PriceLists | PUT /{id} | Updates a price list |
| PriceLists | DELETE /{id} | Soft-deletes a price list (blocked if linked to active SO/contract) |
| PriceLists | POST /{id}/clone | Clones a price list as Draft |

**Checklist:**

- [x] Add `Swashbuckle.AspNetCore` 6.9.0 to `Items.API.csproj`; removed `Microsoft.AspNetCore.OpenApi` (version conflict)
- [x] Enable XML doc generation: `<GenerateDocumentationFile>true</GenerateDocumentationFile>` + `<NoWarn>1591</NoWarn>`
- [x] Registered Swagger in `Program.cs`: `AddSwaggerGen` with `OpenApiInfo`, `IncludeXmlComments`, `UseInlineDefinitionsForEnums`
- [x] Wired `UseSwagger()` + `UseSwaggerUI` in `Program.cs`; added `JsonStringEnumConverter` to controllers JSON options
- [x] Added `[Produces("application/json")]` to all 6 controllers
- [x] Added `[Tags("...")]` to all 6 controllers
- [x] Added `[ProducesResponseType(500)]` at class level on all 6 controllers
- [x] Added XML `<summary>` + `<param>` + `<returns>` to all 31 action methods across all 6 controllers
- [x] `JsonStringEnumConverter` registered; enums render as strings in Swagger
- [x] `dotnet build Items.slnx` → 0 errors, 0 warnings

---

### Phase 10: Data Seeding — Startup Initializer with Sample Data [DONE - 2026-04-14]

**Purpose:** Populate the database with realistic reference data so the API is immediately explorable via Swagger after a fresh `dotnet run`. A `DbInitializer` service checks each table on startup and seeds only if empty — no migration coupling, no fixed IDs in migration history.

**Approach: `DbInitializer` service (recommended over `HasData()`)**
- Registered as a scoped service in `Items.Infrastructure` DI
- Called from `Program.cs` immediately after `await app.MigrateDbAsync()` (or inline scope)
- Each entity block: `if (!await ctx.Table.AnyAsync()) { ctx.AddRange(seedData); }`
- Single `await ctx.SaveChangesAsync()` at the end of each logical group

**Seeding order (FK-safe):**
1. `UnitOfMeasure` (no dependencies)
2. `ItemSettings` (singleton; no dependencies)
3. `ItemClass` (no dependencies)
4. `Item` (→ ItemClass, UnitOfMeasure)
5. `ItemAlternateUom` (→ Item, UnitOfMeasure)
6. `ItemTechnicalSpec` (→ Item)
7. `ItemDrawing` (→ Item)
8. `ItemWarehouseThreshold` (→ Item)
9. `ItemVendorMapping` (→ Item)
10. `ItemVendorPurchaseUom` (→ ItemVendorMapping, UnitOfMeasure)
11. `ItemVendorPricing` (→ ItemVendorMapping)
12. `PriceList` (no dependencies)
13. `PriceListItem` (→ PriceList, Item/ItemClass)

**Sample data summary (realistic values from SRS context):**

*UnitOfMeasure (3 rows):*
- `PCS` — Pieces; individual discrete unit count
- `KG` — Kilograms; weight measurement
- `MT` — Metric Tonne; 1 MT = 1 000 KG; used as bulk steel purchase UOM

*ItemSettings (1 row — singleton):*
- ItemCodeMaxLength: 20 | AllowItemLevelCustomization: true | LogItemClassOverrides: true
- AllowDuplicateNames: false | AllowTagNumberInSalesDocs: true
- EnableAutoBatchSerialGeneration: true | AutoGenPrefix: "BTH" | AutoGenSeparator: "-"
- AutoGenSequenceStart: 1000 | AutoGenSequenceIncrement: 1
- CopyDescription/TechnicalSpecs/PricingDetails/TaxDetails/ItemClassDefaults: true | CopyAttachments: false
- EcnEntryPermission: true | CcnNumberingBasis: ParentItem

*ItemClass (2 rows):*
- `IC-000001` "Steel Raw Materials" — Goods / RM / Purchasable only / FIFO / Batch traceability / InwardNumberTracking + HeatNumberTracking / QC required / IndentMandatory / FastMoving: 200 / SlowMoving: 50 / LeadTimes: 3+2 days / SalesAccount: 4100 / PurchaseAccount: 5100
- `IC-000002` "Electronic Assemblies" — Goods / AS / Saleable only / MovingAverage / Serial traceability / QC+TPI required (PreDispatch) / FastMoving: 500 / SlowMoving: 100 / SalesAccount: 4200 / PurchaseAccount: 5200

*Item (2 rows):*
- `ITM-000001` "Mild Steel Plate 10mm" SKU: `MS-PLATE-10MM` — Class: IC-000001 / BaseUom: KG / Material: Mild Steel IS 2062 / HSN: 72085100 / 18% GST / IN / PurchaseCost: ₹58.50/KG
- `ITM-000002` "Precision PCB Assembly Rev 2" SKU: `PCB-ASM-REV2` — Class: IC-000002 / BaseUom: PCS / Material: FR4 Fibreglass / HSN: 85340000 / 18% GST / IN / SalesPrice: ₹2,450/PCS / MmfgItem: true (min qty 50) / DiscountEligible: true

*ItemAlternateUom (2 rows):*
- Item 1 ↔ MT: ConversionFactor = 1000, IsPerfect = false
- Item 2 ↔ BOX (10 PCS): ConversionFactor = 10, IsPerfect = true

*ItemTechnicalSpec (2 rows):*
- Item 1 — CapabilityType: "Tensile Strength", CapabilityValue: "410 MPa"
- Item 2 — CapabilityType: "Operating Voltage", CapabilityValue: "3.3 V – 5 V DC"

*ItemDrawing (2 rows):*
- Item 1 — "Plate Cutting Layout" / DRW-MS-10-001 / Rev A / A3 / ShowInProductionBooking: false
- Item 2 — "PCB Schematic Rev 2" / DRW-PCB-002-R2 / Rev B / A4 / ShowInProductionBooking: true

*ItemWarehouseThreshold (2 rows):*
- Item 1 / "Main Store" — Min: 500 KG / Max: 5 000 KG / Reorder: 1 000 KG / Opening: 2 000 KG @ ₹57.00 on 2026-04-01
- Item 2 / "FG Warehouse" — Min: 20 PCS / Max: 200 PCS / Reorder: 50 PCS / Opening: 75 PCS @ ₹2 200.00 on 2026-04-01

*ItemVendorMapping (2 rows):*
- `IVM-000001` — Item 1 / VendorId: 101 (Tata Steel Distributors) / VendorItemCode: SS-GRD-A-2062 / Preferred / MOQ: 500 KG / LeadTime: 7 days
- `IVM-000002` — Item 2 / VendorId: 205 (Rexcel Electronics) / VendorItemCode: PCB-V2-EXT / Preferred / MOQ: 10 PCS / LeadTime: 14 days

*ItemVendorPurchaseUom (2 rows):*
- Mapping 1 → MT / Imperfect / ConversionRate: 1000 / Tolerance: 3%
- Mapping 2 → PCS / Perfect / ConversionRate: 1 / Tolerance: 0%

*ItemVendorPricing (2 rows):*
- Mapping 1 — BasePurchasePrice: ₹58 500/MT / INR / Discount: 2% / Effective: ₹57 330 / Valid: 2026-01-01 → 2026-12-31 / Net 30
- Mapping 2 — BasePurchasePrice: ₹2 200/PCS / INR / Discount: ₹50 fixed / Effective: ₹2 150 / Valid: 2026-04-01 → 2026-09-30 / 50% Advance

*PriceList (2 rows):*
- `PL-000001` "Domestic Wholesale 2026" — Sales / Active / Priority: 100 / INR / AllCustomers
- `PL-000002` "Steel Vendor Contract H1 2026" — Purchase / Active / Priority: 80 / INR / AllVendors

*PriceListItem (2 rows — quantity-break slabs for PL-000001 on PCB Assembly):*
- PL-000001 / Item 2 / Qty 1–49 / Base: ₹2 450 / Discount: 5% / Final: ₹2 327.50 / Valid: 2026-01-01 → 2026-12-31
- PL-000001 / Item 2 / Qty 50–∞ / Base: ₹2 450 / Discount: 10% / Final: ₹2 205.00 / Valid: 2026-01-01 → 2026-12-31

**Checklist:**

- [x] Create `Items.Infrastructure/Persistence/DbInitializer.cs` — `IDbInitializer` interface + `DbInitializer` implementation
- [x] Seed `UnitOfMeasure` (4 rows: PCS, KG, MT, BOX — BOX added for PCB alternate UOM)
- [x] Seed `ItemSettings` (1 row — singleton)
- [x] Seed `ItemClass` (2 rows: Steel Raw Materials, Electronic Assemblies)
- [x] Seed `Item` (2 rows: MS Plate 10mm, PCB Assembly Rev 2)
- [x] Seed `ItemAlternateUom` (2 rows — MT for MS Plate; BOX/10PCS for PCB)
- [x] Seed `ItemTechnicalSpec` (2 rows)
- [x] Seed `ItemDrawing` (2 rows)
- [x] Seed `ItemWarehouseThreshold` (2 rows — one per item, one warehouse each)
- [x] Seed `ItemVendorMapping` (2 rows — one per item, preferred vendor each)
- [x] Seed `ItemVendorPurchaseUom` (2 rows)
- [x] Seed `ItemVendorPricing` (2 rows)
- [x] Seed `PriceList` (2 rows: domestic wholesale + vendor contract)
- [x] Seed `PriceListItem` (2 rows — quantity-break slabs on PCB Assembly under wholesale list)
- [x] Register `IDbInitializer` → `DbInitializer` in `Items.Infrastructure` DI (`AddScoped`)
- [x] Call `await scope.ServiceProvider.GetRequiredService<IDbInitializer>().SeedAsync()` in `Program.cs` after DB migration

---

## Session Log

### 2026-04-14 — Phase 10 complete
- Created `Items.Infrastructure/Persistence/DbInitializer.cs` with `IDbInitializer` interface and `DbInitializer` implementation
- Seeds 13 entity types in FK-safe order; each block is idempotent (`AnyAsync` guard — skips if table already has rows)
- Added a 4th UOM row "BOX" (needed for PCB Assembly alternate UOM — not in original tracker spec)
- Resolved `ValuationMethod.Fifo` casing (enum uses `Fifo`, not `FIFO`)
- Registered `IDbInitializer` → `DbInitializer` as `AddScoped` in `Items.Infrastructure` DI
- Wired `SeedAsync()` call in `Program.cs` inside a `using (var scope = ...)` block before middleware pipeline
- `dotnet build Items.Infrastructure` + `dotnet build Items.API` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phases 8 & 9 complete
- Added `AutoMapper` 16.1.1 to Items.Application.csproj
- Registered `AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()))` in Application DI
- Created 6 mapping profiles (one per module) in `Mappings/` subfolders under each domain folder:
  - `ItemSettingsMappingProfile`: 2 maps (Entity→Dto, Request→Entity) — flat 1:1
  - `UomMappingProfile`: 3 maps; `ProjectTo<UomDto>` replaces inline Select in GetAllAsync
  - `ItemClassMappingProfile`: 4 maps; `ProjectTo<ItemClassSummaryDto>` in GetAllAsync; `ItemNature` ignored on Update map
  - `ItemMappingProfile`: 11 maps; ForMember for nav-derived fields (ItemClassName, BaseUomCode, UomCode); IsActive-filtered sub-collections in Item→ItemDto; `ProjectTo<ItemSummaryDto>` in GetAllAsync; UpdateItemRequest→Item ignores ItemNature + dimension flags
  - `VendorMappingMappingProfile`: 8 maps; ForMember for ItemName, PurchaseUomCode; ItemId/VendorId ignored on Update; `ProjectTo<ItemVendorMappingSummaryDto>` in GetAllAsync
  - `PriceListMappingProfile`: 6 maps; LineItems sorted by MinQty in PriceList→PriceListDto; FinalUnitPrice ignored on request map; `ProjectTo<PriceListSummaryDto>` in GetAllAsync
- Rewrote all 6 services: replaced `MapToDto()`, `ApplyRequest()`, `BuildLineItem()` with `_mapper.Map`; removed inline `new Entity { ... }` initializers; added IMapper constructor injection
- Clone operations kept manual per tracker (business logic: code generation, Status=Draft, deep copy)
- Sub-collections with computed fields (EffectivePurchasePrice, FinalUnitPrice, ConversionRate for perfect UOM) still calculated in service after mapping
- Added `Swashbuckle.AspNetCore` 6.9.0 to Items.API.csproj; removed `Microsoft.AspNetCore.OpenApi` (version conflict with Swashbuckle 6.x)
- Enabled XML doc generation and CS1591 suppression in Items.API.csproj
- Updated Program.cs: AddSwaggerGen (OpenApiInfo, IncludeXmlComments, UseInlineDefinitionsForEnums), UseSwagger, UseSwaggerUI; added JsonStringEnumConverter to controller JSON options
- Added [Produces("application/json")], [Tags("...")], [ProducesResponseType(500)] at class level to all 6 controllers
- Added XML <summary>/<param>/<returns> to all 31 action methods across 6 controllers
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 7 complete
- Added `FluentValidation` 11.11.0 + `FluentValidation.DependencyInjectionExtensions` 11.11.0 to Items.Application.csproj
- Added `FluentValidation.AspNetCore` 11.3.0 to Items.API.csproj
- Registered `AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())` in Application DI
- Wired `AddFluentValidationAutoValidation()` in Items.API Program.cs (after AddControllers)
- Created 18 validator files across all 6 entity domains (Validators/ subfolder per domain):
  - ItemSettings: `UpdateItemSettingsRequestValidator` (ItemCodeMaxLength 1–50, AutoGenSequenceIncrement > 0 if set, CcnNumberingBasis enum)
  - Uoms: `CreateUomRequestValidator`, `UpdateUomRequestValidator` (Code/Name required + max lengths)
  - ItemClasses: `CreateItemClassRequestValidator`, `UpdateItemClassRequestValidator` (8-section rules; Service→no inventory; at-least-one Purchasable/Saleable; FastMoving > SlowMoving; TPI/ShelfLife/MinProd/AutoAlloc conditionals)
  - Items: `AlternateUomRequestValidator`, `TechnicalSpecRequestValidator`, `DrawingRequestValidator`, `WarehouseThresholdRequestValidator` (shared child validators); `CreateItemRequestValidator`, `UpdateItemRequestValidator` (all section rules + RuleForEach for sub-entities)
  - ItemVendorMappings: `PurchaseUomRequestValidator`, `VendorPricingRequestValidator`, `CreateItemVendorMappingRequestValidator`, `UpdateItemVendorMappingRequestValidator`
  - PriceLists: `PriceListItemRequestValidator`, `CreatePriceListRequestValidator`, `UpdatePriceListRequestValidator`
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 6 complete
- Created 4 enums in Domain/Enums/: `PriceListApplicableTo`, `PriceListStatus`, `PriceListTargetType`, `PricingScopeType`
- Created 2 entities in Domain/Entities/: `PriceList` (header with code, applicableTo, name, status, priority, currency, targetType, targetIds JSON, pricingScopeType), `PriceListItem` (slab with item/class FKs, qty range, base price, discount, computed final price, rounding, validity)
- Created DTOs in Application/PriceLists/Dtos/: summary, full, line-item, create/update/clone request DTOs
- Created `IPriceListService` + `PriceListService` — paginated list (ordered by priority desc), CRUD, clone; name uniqueness enforced; FinalUnitPrice computed from BasePrice + Discount; clones start as Draft; code pattern `PL-000001`; line items sorted by MinQty in GET response
- Added 2 DbSets to `IItemsDbContext` and `ItemsDbContext`
- Created 2 EF configurations with FKs, precision on decimal fields, enum-to-string conversions, cascade delete for line items (Infrastructure/Persistence/Configurations/)
- Registered `IPriceListService` → `PriceListService` in Application DI
- Created `PriceListsController` (API/Controllers/) — GET (paginated), GET by id, POST, PUT, DELETE, POST clone at `/api/v1/price-lists`
- `dotnet ef migrations add AddPriceLists` → succeeded
- `dotnet ef database update` → Done
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 5 complete
- Created 3 enums in Domain/Enums/: `DiscountType`, `WarrantyDurationUnit`, `WarrantyStartBasis`
- Created 3 entities in Domain/Entities/: `ItemVendorMapping` (code, item/vendor FKs, logistics, warranty), `ItemVendorPurchaseUom` (UOM grid with conversion rate/tolerance), `ItemVendorPricing` (base price, discount, effective price, validity, payment terms)
- Created DTOs in Application/ItemVendorMappings/Dtos/: summary, full, create/update/clone request DTOs + sub-entity DTOs
- Created `IItemVendorMappingService` + `ItemVendorMappingService` — paginated+filtered list, CRUD, clone; preferred vendor enforcement (auto-clears previous preferred); effective price computed from base price + discount; code pattern `IVM-000001`
- Added 3 DbSets to `IItemsDbContext` and `ItemsDbContext`
- Created 3 EF configurations with FKs, precision, cascade rules (Infrastructure/Persistence/Configurations/)
- Registered `IItemVendorMappingService` → `ItemVendorMappingService` in Application DI
- Created `ItemVendorMappingsController` (API/Controllers/) — GET (paginated+filtered), GET by id, POST, PUT, DELETE, POST clone at `/api/v1/item-vendor-mappings`
- `dotnet ef migrations add AddItemVendorMapping` → succeeded
- `dotnet ef database update` → Done
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 4 complete
- Created 6 new enums in Domain/Enums/: `TaxPreference`, `ConsumptionType`, `ConsumptionStartBasis`, `ConsumptionPeriodUnit`, `FrequencyUnit`, `PostExhaustionAction`
- Created 5 entities in Domain/Entities/: `Item` (8 SRS sections, 70+ fields), `ItemAlternateUom`, `ItemTechnicalSpec`, `ItemDrawing`, `ItemWarehouseThreshold`
- Created DTOs in Application/Items/Dtos/: `ItemSummaryDto`, `ItemDto`, `CreateItemRequest`, `UpdateItemRequest`, `CloneItemRequest` + 4 sub-entity request/response DTOs
- Created `IItemService` + `ItemService` (Application/Items/) — paginated+filtered list, CRUD, clone with deep-copy of all sub-entities; ItemNature/dimension flags non-editable on update per SRS; SKU uniqueness enforced
- Added 5 DbSets to `IItemsDbContext` and `ItemsDbContext`
- Created 5 EF configurations with FKs, precision, and cascade rules (Infrastructure/Persistence/Configurations/)
- Registered `IItemService` → `ItemService` in Application DI
- Created `ItemsController` (API/Controllers/) — GET (paginated+filtered), GET by id, POST, PUT, DELETE, POST clone at `/api/v1/items`
- `dotnet ef migrations add AddItemMaster` → succeeded
- `dotnet ef database update` → Done
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 3 complete
- Created 7 enums in Domain/Enums/: `ItemNature`, `ItemType`, `ValuationMethod`, `TraceabilityLevel`, `IssueMethod`, `TpiTriggerStage`, `AutoAllocationOn`
- Created `ItemClass` entity (Domain/Entities/) — all 8 SRS sections (65 fields)
- Created `PagedResult<T>` helper in Application/Common/
- Created DTOs: `ItemClassSummaryDto`, `ItemClassDto`, `CreateItemClassRequest`, `UpdateItemClassRequest`, `CloneItemClassRequest` (Application/ItemClasses/Dtos/)
- Created `IItemClassService` + `ItemClassService` (Application/ItemClasses/) — paginated list, CRUD, clone; ItemNature non-editable on update per SRS
- Added `DbSet<ItemClass>` to `IItemsDbContext` and `ItemsDbContext`
- Added `ItemClassConfiguration` EF config with unique index on Code, precision on decimal fields (Infrastructure/Persistence/Configurations/)
- Registered `IItemClassService` → `ItemClassService` in Application DI
- Created `ItemClassesController` (API/Controllers/) — GET (paginated), GET by id, POST, PUT, DELETE, POST clone at `/api/v1/item-classes`
- `dotnet ef migrations add AddItemClass` → succeeded
- `dotnet ef database update` → Done
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors



### 2026-04-14 — Phase 2 complete
- Created `UnitOfMeasure` entity (Domain/Entities/)
- Created DTOs: `UomDto`, `CreateUomRequest`, `UpdateUomRequest` (Application/Uoms/Dtos/)
- Created `IUomService` + `UomService` (Application/Uoms/) — full CRUD, code uniqueness check, soft-delete
- Added `DbSet<UnitOfMeasure>` to `IItemsDbContext` and `ItemsDbContext`
- Added `UnitOfMeasureConfiguration` EF config with unique index on Code (Infrastructure/Persistence/Configurations/)
- Registered `IUomService` → `UomService` in Application DI
- Created `UomsController` (API/Controllers/) — GET all, GET by id, POST, PUT, DELETE at `/api/v1/uoms`
- `dotnet ef migrations add AddUnitOfMeasure` → succeeded
- `dotnet ef database update` → Done
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 1 complete
- Created `CcnNumberingBasis` enum (Domain/Enums/)
- Created `ItemSettings` entity (Domain/Entities/) — 4 SRS sections, inherits BaseEntity
- Created DTOs: `ItemSettingsDto`, `UpdateItemSettingsRequest` (Application/ItemSettings/Dtos/)
- Created `IItemSettingsService` + `ItemSettingsService` (Application/ItemSettings/) — get-or-default + upsert pattern
- Added `DbSet<ItemSettings>` to `IItemsDbContext` and `ItemsDbContext`
- Added `ItemSettingsConfiguration` EF config (Infrastructure/Persistence/Configurations/)
- Registered `IItemSettingsService` → `ItemSettingsService` in Application DI
- Created `ItemSettingsController` (API/Controllers/) — GET + PUT at `/api/v1/item-settings`
- `dotnet ef migrations add AddItemSettings` → succeeded
- `dotnet ef database update` → Done
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors

### 2026-04-14 — Phase 0 complete
- Read SRS pages: item-master.md, item-class.md, item-settings.md, item-vendor-mapping.md, price-lists.md
- Read GRAPH_REPORT.md (54 nodes, 11 communities — Item Master and Item Class are god nodes)
- Read development-standards.md (naming, async, DTOs, Serilog, ProblemDetails, /api/v1/ versioning)
- Revised tracker phases: replaced generic Item/Category/UOM with SRS-accurate entities
  (ItemSettings, UOM, ItemClass, Item+sub-entities, ItemVendorMapping, PriceList)
- Scaffolded solution: Items.slnx, 4 projects (Domain/Application/Infrastructure/API)
- Project references: Domain ← Application ← Infrastructure ← API (clean dependency flow)
- Packages: EF Core 10 SQL Server + Tools (Infrastructure), EF Core Design + Serilog.AspNetCore (API)
- Created: BaseEntity, DomainException hierarchy, IItemsDbContext, ItemsDbContext with audit hook
- Created: ExceptionHandlingMiddleware (→ ProblemDetails), Serilog in Program.cs
- Connection string configured in appsettings.json
- `dotnet ef migrations add InitialCreate` → succeeded
- `dotnet ef database update` → Done (DB connection verified, __EFMigrationsHistory created)
- `dotnet build Items.slnx` → Build succeeded, 0 warnings, 0 errors
