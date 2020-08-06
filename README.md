# PaunPacker - Texture Atlas Generator

This repository contains sources for a texture atlas generator that I have created as a part of my bachelor thesis. 

PaunPacker is an extensible application for packing textures into texture atlases, that could then be used in 2D game development. The extensibility lies in the possibility to create and import plugins, containing algorithms for packing, image processing, and metadata exporting. The ability to extend the application by means of plugins makes our application also suitable for testing of newly invented algorithms or for testing of custom variations of the existing ones.

The software solution includes application with user interface that allows the user to create texture atlases and perform additional processing of the textures. 
 
Several packing algorithms are currently implemented, namely:
 - Bottom-left algorithm
 - Skyline algorithm
 - Guillotine algorithm
 - Genetic-based algorithm.
 - Maximal rectangles algorithm (MaxRects)
  
All these algorithms can be used as a starting point when developing new plugins. Custom packing algorithms could also be benchmarked using the benchmark runner that is a part of the solution.

In addition to generating texture atlases, this application can also generate metadata, that can then be imported by supported game frameworks or libraries. The process of metadata serialization is also customizable, and so users can supply the application with serializers that produce metadata that matches their needs.

For more information about the PaunPacker, including a detailed description of its architecture, user documentation, description of the implemented algorithms, tutorial for plugin development and others, please see the original text of my bachelor thesis that is available <a href="https://github.com/pdokoupil/bachelor-thesis/raw/master/thesis.pdf">here</a>.