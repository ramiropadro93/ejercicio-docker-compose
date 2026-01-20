# Pipeline de procesamiento distribuido con Docker Compose

Este proyecto demuestra un **pipeline de procesamiento distribuido** donde tres microservicios se comunican entre sí para procesar un archivo secuencialmente.

## 📋 **Descripción del Ejercicio**

El ejercicio simula un pipeline donde:
1. **Proyecto1**: Crea un archivo en un volumen compartido y notifica al Proyecto2
2. **Proyecto2**: Lee el archivo, lo procesa y notifica al Proyecto3  
3. **Proyecto3**: Lee el archivo, lo procesa y completa el pipeline


## **Tecnologías utilizadas**

- **.NET 8** - Framework de desarrollo
- **Docker Compose** - Orquestación de contenedores
- **Volúmenes Docker** - Almacenamiento compartido

## **Cómo Ejecutar**

### 1. **Construir y levantar los servicios**
```bash
docker-compose up --build
```

### 2. **Ejecutar el pipeline completo**
```bash
# Iniciar el pipeline enviando POST a Proyecto1
curl -X POST http://localhost:5001/create-file
```

### 3. **Ver el resultado final**
```bash
# Leer el archivo procesado
curl http://localhost:5003/read-file
```

## **Comunicación entre servicios**

Los servicios se comunican usando los nombres de contenedor en la red Docker:
- `http://proyecto2/process-file`
- `http://proyecto3/finalize-file`


## **Solución de problemas**

### **Error de conexión entre servicios**
- Verificar que la red `red-prueba` esté creada
- Confirmar que los nombres de contenedor sean correctos
