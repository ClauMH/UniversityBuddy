CREATE DATABASE UniversityBuddy;
GO
USE UniversityBuddy;
GO

CREATE TABLE Usuarios(
	ID_Usuario INT IDENTITY NOT NULL,
	NomUsuario NVARCHAR(10) NOT NULL,
	Nombre NVARCHAR(100) NOT NULL,
	Contrasena NVARCHAR(100) NOT NULL,
	Universidad NVARCHAR(100),
	FotoPerfil NVARCHAR(100),
	CONSTRAINT PK_Usuarios PRIMARY KEY (ID_Usuario)
);
GO
CREATE TABLE Materias(
	ID_Materia INT IDENTITY NOT NULL,
	ID_Usuario INT NOT NULL,
	Nombre NVARCHAR(100) NOT NULL,
	Maestro NVARCHAR(100),
	Contacto NVARCHAR(100),
	Aula NVARCHAR(100),
	Horario TIME,
	ImagenMateria NVARCHAR(100),
	CONSTRAINT PK_Materia PRIMARY KEY (ID_Materia),
	CONSTRAINT FK_MateriaUsuario FOREIGN KEY (ID_Usuario) REFERENCES Usuarios(ID_Usuario)
);
GO
CREATE TABLE Actividades(
	ID_Actividad INT IDENTITY NOT NULL,
	ID_Usuario INT NOT NULL,
	Nombre NVARCHAR(100) NOT NULL,
	Categoria NVARCHAR(100) NOT NULL,
	ID_Materia INT NOT NULL,
	Descripcion NVARCHAR(300),
	Fecha DATE NOT NULL,
	Estado NVARCHAR(100) NOT NULL,
	CONSTRAINT PK_Actividad PRIMARY KEY (ID_Actividad),
	CONSTRAINT FK_ActividadUsuario FOREIGN KEY (ID_Usuario) REFERENCES Usuarios(ID_Usuario),
	CONSTRAINT ID_Materia FOREIGN KEY (ID_Materia) REFERENCES Materias(ID_Materia)
);
GO

