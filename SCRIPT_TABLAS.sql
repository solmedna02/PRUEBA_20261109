CREATE TABLE Colaboradores (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Correo VARCHAR(100) NOT NULL,
    DepartamentoId INT NULL
);

CREATE TABLE Departamentos (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    ResponsableId INT NULL,
    PresupuestoAnual DECIMAL(12,2) NOT NULL
);

ALTER TABLE Colaboradores
ADD CONSTRAINT FK_Colaborador_Departamento
FOREIGN KEY (DepartamentoId)
REFERENCES Departamentos(Id);

ALTER TABLE Departamentos
ADD CONSTRAINT FK_Departamento_Responsable
FOREIGN KEY (ResponsableId)
REFERENCES Colaboradores(Id);

CREATE TABLE Solicitudes (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    DepartamentoId INT NOT NULL,
    SolicitanteId INT NOT NULL,
    FechaCreacion DATETIME2 DEFAULT GETDATE(),
    Justificacion VARCHAR(500) NOT NULL,
    Estado VARCHAR(30) DEFAULT 'Borrador',
    Total DECIMAL(12,2) DEFAULT 0,
    AprobacionDepartamento VARCHAR(20) DEFAULT 'Pendiente',
    AprobacionFinanzas VARCHAR(20) DEFAULT 'Pendiente',
    AprobacionGerencia VARCHAR(20) DEFAULT 'Pendiente',

    FOREIGN KEY (DepartamentoId)
        REFERENCES Departamentos(Id),

    FOREIGN KEY (SolicitanteId)
        REFERENCES Colaboradores(Id)
);

CREATE TABLE DetallesSolicitud (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SolicitudId INT NOT NULL,
    Descripcion VARCHAR(200) NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(12,2) NOT NULL
        CHECK (PrecioUnitario >= 0),

    FOREIGN KEY (SolicitudId)
        REFERENCES Solicitudes(Id)
);

CREATE TABLE Cotizaciones (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SolicitudId INT NOT NULL,
    Proveedor VARCHAR(150) NOT NULL,
    FechaVigencia DATE NOT NULL,
    CondicionesPago VARCHAR(200),

    FOREIGN KEY (SolicitudId)
        REFERENCES Solicitudes(Id)
);