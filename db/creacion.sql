CREATE TABLE RolUsuario (
    id_rol INTEGER PRIMARY KEY,
    descripcion VARCHAR(50) NOT NULL
);
CREATE TABLE Usuario(
    id_usuario INTEGER PRIMARY KEY AUTOINCREMENT,
    nombre_de_usuario VARCHAR(100) NOT NULL UNIQUE,
    password VARCHAR(50) NOT NULL,
    rol_usuario INTEGER NOT NULL,
    
    CONSTRAINT fk_rol_usuario FOREIGN KEY (rol_usuario) REFERENCES RolUsuario (id_rol)
    ON DELETE no action
    ON UPDATE cascade
);
CREATE TABLE Tablero(
    id_tablero INTEGER PRIMARY KEY AUTOINCREMENT,
    id_usuario_propietario INT NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(250) DEFAULT 'Sin descripción',
    
    CONSTRAINT fk_usuario FOREIGN KEY (id_usuario_propietario) REFERENCES Usuario(id_usuario)
    ON DELETE cascade
    ON UPDATE cascade
);
CREATE TABLE Estado(
    id_estado INTEGER PRIMARY KEY,
    estado VARCHAR(50) NOT NULL UNIQUE
);
CREATE TABLE Color(
    id_color INTEGER PRIMARY KEY,
    color VARCHAR(50) NOT NULL UNIQUE
);
CREATE TABLE Tarea(
    id_tarea INTEGER PRIMARY KEY AUTOINCREMENT,
    id_tablero INT NOT NULL,
    nombre VARCHAR(100) NOT NULL,
    id_estado INTEGER NOT NULL,
    descripcion VARCHAR(250) DEFAULT 'Sin descripción',
    id_color INT NOT NULL,
    id_usuario_asignado INT,
    
    CONSTRAINT fk_tablero FOREIGN KEY (id_tablero) REFERENCES Tablero(id_tablero)
    ON DELETE cascade
    ON UPDATE cascade
    
    CONSTRAINT fk_estado FOREIGN KEY (id_estado) REFERENCES Estado(id_estado)
    ON DELETE cascade
    ON UPDATE cascade
    
    CONSTRAINT fk_color FOREIGN KEY (id_color) REFERENCES Color(id_color)
    ON DELETE cascade
    ON UPDATE cascade
    
    CONSTRAINT fk_usuario_asignado FOREIGN KEY ( id_usuario_asignado) REFERENCES Usuario(id_usuario)
    ON DELETE set null
    ON UPDATE cascade
);



