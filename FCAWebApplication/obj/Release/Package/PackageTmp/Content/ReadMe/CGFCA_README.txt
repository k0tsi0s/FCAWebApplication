CG-FCA v7, Copyright Simon Andrews, Sheffield Hallam University, 2017. Email: s.andrews@shu.ac.uk

Developed in collaboration with Simon Polovina, Sheffield Hallam University, Email: s.polovina@shu.ac.uk

CG-FCA is a Windows-based tool that converts Conceptual Graphs (CGs) into Formal Contexts (for Formal Concept Analysis (FCA))
This new version (v7) also accepts 3 column csv input.

Input: A CG in the standard .cgif file format or 3 column csv.
Output: A Formal Context in the standard .cxt format and a report of graph features (inputs, outputs, direct pathways, cycles, multiple routes).

The conversion is carried out by converting CG triples (Source Concept, Relation, Target Concept) into FCA attribute, object binaries ((Source Concept, Relation), Target Concept). For 3 column csv, although CG-FCA will work with any data values, it makes sense, from a directed graph point of view, that values in the 1st and 3rd column are of the same type and in the 2nd column is something that connects elements in the 1st column with those in the 3rd. E.g., <Entity, Relation, Entity>.

The pathways in the CG are maintained in the Formal Context by creating binaries for each subsequent indirect Target Concept that can be reached from an initial (Source Concept, Relation).

Thus the CG: [C1]->(R1)->[C2]->(R2)->[C3] will result in the following binaries: ((C1,R1),C2), ((C1,R1),C3), ((C2,R2),C3).

The corresponding Formal Context is:

	(C1,R1)	¦ (C2,R2)
-------------------------
   C1		¦
   C2      X	¦
   C3      X	¦    X
   
   
Additionally, CG-FCA automatically reports certain features of a CG:

1) Inputs - CG Source Concepts that are not Targets.
2) Outputs - CG Target Concepts that are not Sources.
3) Direct pathways - from an input to an output (excluding any cyles in between).
4) Cycles.
5) Multiple routes - CG-FCA will report the number of direct pathways from an 'input concept-relation-target concept' to an output concept, when there is more than one route.

The Formal Context produced by CG-FCA can be visualised as a Formal Concept Lattice in tools such as Concept Explorer (ConExp). 

The Concept Lattice is a convenient way to explore the pathways and dependencies in the original CG and highlights features of CGs such as input CG concepts, output CG concepts and the effects of Concept and Relation co-referent links. CG-FCA performs the joins.

The lattice aligns the pathways in a CG from input CG concepts at the top of the lattice to output CG concepts at the bottom.

Note that CG-FCA does not yet support Peirce's Cuts.

CG-FCA is released under the Free and Open Source Software, MIT License. However, the author politely requests that users of the Software please inform him (by email: s.andrews@shu.ac.uk) what use they have made or are making of the Software. This is purely to help the author record what impact the Software has had, whether in private use, commercial application, internal/non-commercial application, research or public benefit. Thank you.
