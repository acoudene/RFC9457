# Objectif

Mettre en avant le respect de la norme RFC9457 en .Net.

# Synthèse de la norme RFC9457

## Référence
https://www.rfc-editor.org/rfc/rfc9457.html

## But

L'objectif principal de la RFC9457 est de normaliser la gestion du détail des "problèmes" rencontrés avec une API en http.
La spécification sert à définir des formats d'erreur communs pour les applications qui en ont besoin afin qu'elles ne soient pas obligées de définir les leurs ou, pire, tentées de redéfinir la sémantique des codes de statut HTTP existants

## Contenu

Type de fichier : JSON
Type de media : application/problem+json
Format de base : 
  - **type (URI)** : Identifie le type de problème spécifique avec un URI, permettant aux API HTTP d'utiliser des URI sous leur contrôle pour identifier des problèmes qui leur sont spécifiques ou de réutiliser des URI existants pour faciliter l'interopérabilité RFC Editor. 2 options possibles :
    - URL déréférençable : Elle doit retourner de la documentation HTML
    - Tag URI : Simple identifiant unique, non déréférençable (ex: urn:problem-type:product-not-found)
  - **title (string)** : Un résumé court et lisible du type de problème
  - **status (number)** : Le code de statut HTTP généré par le serveur
  - **detail (string)** : Une explication détaillée spécifique à cette occurrence du problème
  - **instance (URI)** : Une URI identifiant l'occurrence spécifique du problème, ce qui peut être utile à des fins de support ou de forensics RFC Editor

## Points importants

Les détails de problème ne sont pas un outil de débogage pour l'implémentation sous-jacente ; il s'agit plutôt d'un moyen d'exposer plus de détails sur l'interface HTTP elle-même.
Les URI de type doivent être stables dans le temps et sous le contrôle de l'organisation.
Le format standard évite la nécessité de créer de nouveaux formats de réponse d'erreur pour chaque API.

# Concrètement en .Net 9

