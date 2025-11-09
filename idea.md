If gestor de alumnos:
	Usuarios:
		-preceptor (maneja la carga de los estudiantes) y secretario. usuario y contraseña.
		-familias (ven datos publicos, no pueden hacer cambios)
	}cambios
	primer cambio




# ?? Sistema de Gesti車n de Turnos - Proyecto Integral

Este proyecto propone el desarrollo de un **sistema completo de gesti車n de turnos m谷dicos**, con una arquitectura moderna basada en principios de **Clean Architecture / Hexagonal**, y diferentes interfaces de usuario que consumen una **API REST centralizada desarrollada en C#**.

> Este sistema est芍 dise?ado como proyecto final de la tecnicatura en programaci車n, pero tambi谷n forma parte de un portfolio profesional orientado al desarrollo backend y arquitecturas escalables.

---

## ?? Arquitectura General

- **Backend / API REST** en C# (.NET Core)
  - Arquitectura limpia / hexagonal
  - Control de acceso por roles (JWT)
  - Adaptadores desacoplados (Base de datos, Web, Escritorio)
- **Frontend escritorio** (WPF)
  - Modo online y **soporte offline** con sincronizaci車n posterior
  - Enfocado en roles operativos (secretar赤a, m谷dicos)
- **Frontend web**
  - Versi車n Razor Pages o SPA con React (a definir)
  - Enfocado en clientes (pedir turnos) y jefatura (reportes)

---

## ?? Objetivos del Proyecto

- Aplicar conceptos de an芍lisis de sistemas:
  - Detecci車n de actores y necesidades organizacionales
  - Dise?o de roles, accesos y procesos
- Consolidar conocimientos t谷cnicos en:
  - Programaci車n orientada a objetos
  - Servicios web y APIs REST
  - Persistencia de datos y sincronizaci車n offline
  - Arquitecturas desacopladas y escalables
- Generar un sistema reutilizable y extensible

---

## ?? Perfiles de Usuario

- **Secretar赤a**: crea y gestiona turnos
- **M谷dico**: accede al historial cl赤nico de sus pacientes
- **Jefatura / Administraci車n**: consulta reportes y estad赤sticas
- **Cliente / Paciente** (opcional): solicita turnos desde el frontend web

---

## ?? M車dulos Funcionales

- Gesti車n de turnos (alta, baja, reprogramaci車n)
- Registro y b迆squeda de pacientes
- Historia cl赤nica (solo m谷dicos)
- Reportes y estad赤sticas (solo administraci車n)
- Autenticaci車n y autorizaci車n (JWT)
- Sincronizaci車n offline para escritorio

---

## ?? Roadmap

### Etapa 1 - N迆cleo + API REST
- [ ] Modelado de entidades (`Turno`, `Paciente`, `Usuario`, etc.)
- [ ] Implementaci車n de casos de uso (`AsignarTurno`, `CancelarTurno`, etc.)
- [ ] API RESTful protegida con JWT
- [ ] Tests unitarios del dominio

### Etapa 2 - Cliente de Escritorio
- [ ] Pantallas para secretar赤a y m谷dicos
- [ ] Modo conectado
- [ ] Modo offline con cache local (ej. SQLite + registro de operaciones)
- [ ] Sincronizaci車n autom芍tica al recuperar conexi車n

### Etapa 3 - Interfaz Web
- [ ] Cliente web con login
- [ ] Dashboard para administraci車n
- [ ] Solicitud de turnos por parte del paciente (opcional)
- [ ] Despliegue en entorno gratuito (Render, Railway, etc.)

---

## ?? Tecnolog赤as y Herramientas

- **Backend**
  - C#, .NET 8+
  - Entity Framework Core / Dapper
  - JWT Authentication
  - Clean Architecture

- **Frontend Escritorio**
  - WPF (.NET)
  - SQLite (modo offline)
  - MVVM

- **Frontend Web**
  - Razor Pages (alternativa: React + fetch)
  - Bootstrap o Tailwind (dise?o)
  - ASP.NET Core Identity o JWT simple

- **Extras**
  - GitHub Actions (CI/CD)
  - Docker (opcional)
  - Diagramas con draw.io o Mermaid

---

## ?? Estructura de Carpetas (tentativa)
/TurnosApp
念岸岸 /Domain # Entidades y puertos (interfaces)
念岸岸 /Application # Casos de uso y l車gica de negocio
念岸岸 /Infrastructure # Implementaciones concretas (repositorios, DB, servicios externos)
念岸岸 /WebApi # Adaptador HTTP: controladores, validaciones, auth
念岸岸 /DesktopClient # Proyecto WPF
念岸岸 /WebClient # Razor Pages o React
弩岸岸 /Docs # Diagramas, documentaci車n t谷cnica y funcional


---

## ?? Estado actual

> Proyecto en desarrollo (julio 2025).  
> Actualmente se encuentra en etapa de dise?o del dominio y planificaci車n de los casos de uso principales.

---

## ?? Licencia

MIT License 〞 libre para uso educativo o comercial con atribuci車n.

---

## ?? Autor

Desarrollado por **Alexander Seling**  
Estudiante de la Tecnicatura en Programaci車n y Analista de Sistemas  
Contacto: [GitHub](https://github.com/tuusuario) | [LinkedIn](https://linkedin.com/in/tuusuario)

