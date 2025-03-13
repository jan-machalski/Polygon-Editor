# Polygon & Curvilinear Polygon Editor

## Overview
This application is a **polygon and curvilinear polygon editor** that allows users to create, edit, and manage **single polygons** with **Bezier curve segments**. The editor provides full control over vertices, edges, constraints, and continuity settings.

---

## Features
### **Polygon Editing**
- **Create and delete polygons** (only one can exist at a time).
- **Edit polygons by modifying vertices and edges**:
  - Move vertices.
  - Delete vertices.
  - Add a new vertex in the middle of an edge.
  - Move the entire polygon.
 
![image](https://github.com/user-attachments/assets/b48323ce-a365-43bf-b4b2-a9baf006cd6a)


### **Edge Constraints & Relations**
- Add constraints to selected edges:
  - **Horizontal edge** constraint.
  - **Vertical edge** constraint.
  - **Fixed edge length** (modifiable in an edit window).
- Constraints are **visually indicated** near the middle of the edge.
- **Rules for constraints**:
  - Maximum **one constraint per edge**.
  - Adjacent edges **cannot both be horizontal or both be vertical**.
  - Adding or removing vertices **removes constraints on affected edges**.
- Constraints can be removed manually.

### **Bezier Curve Integration**
- Convert any edge into a **third-degree Bezier curve**.
- Bezier curve representation:
  - **Control polygon (dashed line).**
  - **Two control points per segment.**
- Editing Bezier curves:
  - Move control points.
  - Adjust vertex positions while preserving shape constraints.
- **Edge continuity settings**:
  - **G0** (positional continuity).
  - **G1** (tangent unit vector continuity).
  - **C1** (full tangent vector continuity).

---

## User Interface
- **Graphical representation** of polygons and Bezier curves.
- **Interactive vertex & edge selection**.
- **Icons for constraints** to improve visual clarity.
- **Real-time updates** when moving or modifying elements.

