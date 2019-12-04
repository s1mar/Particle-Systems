# Particle Systems

### Description
 * A means for of modeling fuzzy objects, such as fire,
clouds, smoke, water, etc.
    - Don’t have smooth well-defined surfaces
    - non-rigid objects

 * Term coined by William Reeves who first used particle
systems in “Star Trek II”

### Modeling
* An object is not represented by a set of
primitive surface elements, but as clouds of
primitive particles that define its volume.

* A particle system is not a static entity, its
particles change form and move.

* An object represented by a particle system is
not deterministic, its shape and form is not
completely specified. Stochastic!

### Advantages
* Complex systems can be created with little human effort.
* The level of detail can be easily adjusted.

### Basic Model
* New particles are generated
* Each new particle is assigned its own set of attributes
* Any particles that have existed for a predetermined time are
destroyed
* The remaining particles are transformed and moved according
to their dynamic attributes
* An image of the remaining particles is rendered


 ### Demo
 ![](psdemo.gif)  
 If gif not viewable, please checkout the YouTube Link: <https://youtu.be/eslsMUzghZ4>  