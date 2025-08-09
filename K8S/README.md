# K8S - Notas de Ingress y verificación local

Este entorno usa ingress-nginx y expone el controlador en `localhost` (Docker Desktop). El Ingress resuelve el host `acme.com` y enruta a los servicios internos.

## Manifiestos relevantes
- `platforms-depl.yaml`: Deployment + Service `platforms-clusterip-srv` (puerto 80 -> targetPort 8080)
- `commands-depl.yaml`: Deployment + Service `commands-clusterip-srv` (puerto 80 -> targetPort 8080)
- `ingress-srv.yaml`: Ingress con `ingressClassName: nginx` y reglas para `acme.com`

## Aplicar cambios
Ejecuta:

```powershell
kubectl apply -f D:\Work\Repositories\PlatformMicros\K8S\platforms-depl.yaml
kubectl apply -f D:\Work\Repositories\PlatformMicros\K8S\commands-depl.yaml
kubectl apply -f D:\Work\Repositories\PlatformMicros\K8S\ingress-srv.yaml
```

## Configurar el host local (Windows)
Agregar en `C:\Windows\System32\drivers\etc\hosts` la línea:

```
127.0.0.1 acme.com
```

Con esto, podrás acceder a `http://acme.com` directamente en el navegador.

## Verificar el Ingress y servicios

- Describir el Ingress:
```powershell
kubectl describe ingress ingress-srv -n default
```
Debe mostrar `Ingress Class: nginx` y las reglas:
- `/api/platforms` -> `platforms-clusterip-srv:80` (endpoint 8080)
- `/api/c/platforms` -> `commands-clusterip-srv:80` (endpoint 8080)

- Ver endpoints y pods:
```powershell
kubectl get endpoints platforms-clusterip-srv -o wide
kubectl get pods -l app=platformservice -o wide
```

## Probar rutas
- Con curl (forzando el host header):
```powershell
curl.exe -i -H "Host: acme.com" http://localhost/api/platforms/
```
Debería responder `200 OK` con JSON de plataformas.

- Desde navegador (una vez editado `hosts`):
```
http://acme.com/api/platforms/
```

Para `commands`:
```powershell
curl.exe -i -H "Host: acme.com" http://localhost/api/c/platforms
```

## Notas
- Si usas Minikube, sustituye `127.0.0.1` por la IP de `minikube ip` en el archivo `hosts`.
- Si despliegas en otro entorno con LoadBalancer, apunta `acme.com` a la `EXTERNAL-IP` del servicio `ingress-nginx-controller`.
- El `pathType: Prefix` acepta con o sin barra final (`/api/platforms` y `/api/platforms/`).

