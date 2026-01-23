# Ejemplos de JSON para Postman

## Base URL
```
http://localhost:5207
```

---

## 1. POST /expenses - Crear un gasto

**URL:** `POST http://localhost:5207/expenses`

**Headers:**
```
Content-Type: application/json
```

**Body (raw JSON):**
```json
{
  "amount": 100,
  "categoryName": "Food"
}
```

**Respuesta esperada:** 201 Created
```json
{
  "id": 1,
  "amount": 100,
  "categoryName": "Food"
}
```

---

## 2. GET /expenses - Obtener todos los gastos

**URL:** `GET http://localhost:5207/expenses`

**Headers:** (ninguno necesario)

**Body:** (ninguno)

**Respuesta esperada:** 200 OK
```json
[
  {
    "id": 1,
    "amount": 100,
    "categoryName": "Food"
  }
]
```

---

## 3. GET /expenses/{id} - Obtener un gasto por ID

**URL:** `GET http://localhost:5207/expenses/1`

**Headers:** (ninguno necesario)

**Body:** (ninguno)

**Respuesta esperada:** 200 OK
```json
{
  "id": 1,
  "amount": 100,
  "categoryName": "Food"
}
```

O 404 Not Found si no existe.

---

## 4. PUT /expenses/{id} - Actualizar un gasto

**URL:** `PUT http://localhost:5207/expenses/1`

**Headers:**
```
Content-Type: application/json
```

**Body (raw JSON):**
```json
{
  "amount": 250,
  "categoryName": "Updated Food"
}
```

**Respuesta esperada:** 200 OK
```json
{
  "id": 1,
  "amount": 250,
  "categoryName": "Updated Food"
}
```

O 404 Not Found si no existe.

---

## 5. DELETE /expenses/{id} - Eliminar un gasto

**URL:** `DELETE http://localhost:5207/expenses/1`

**Headers:** (ninguno necesario)

**Body:** (ninguno)

**Respuesta esperada:** 204 No Content (sin body)

O 404 Not Found si no existe.

---

## Secuencia de Prueba Recomendada

1. **Crear un gasto** (POST /expenses)
   - Copia el `id` de la respuesta

2. **Ver todos los gastos** (GET /expenses)
   - Verifica que el gasto creado esté en la lista

3. **Obtener el gasto por ID** (GET /expenses/{id})
   - Usa el ID del paso 1

4. **Actualizar el gasto** (PUT /expenses/{id})
   - Usa el ID del paso 1
   - Cambia amount y categoryName

5. **Verificar la actualización** (GET /expenses/{id})
   - Verifica que los valores cambiaron

6. **Eliminar el gasto** (DELETE /expenses/{id})
   - Usa el ID del paso 1

7. **Verificar la eliminación** (GET /expenses/{id})
   - Debería retornar 404 Not Found
