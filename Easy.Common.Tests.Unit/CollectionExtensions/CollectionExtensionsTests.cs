namespace Easy.Common.Tests.Unit.CollectionExtensions;

using Easy.Common.Extensions;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;

[TestFixture]
internal sealed class CollectionExtensionsTests
{
    [Test]
    public void When_generating_a_tree()
    {
        var someData = new[]
        {
            new Category(1, 0, "Sport"),
            new Category(2, 1, "Balls"),
            new Category(3, 1, "Shoes"),
            new Category(4, 0, "Electronics"),
            new Category(5, 4, "Cameras"),
            new Category(6, 5, "Lenses"),  
            new Category(7, 5, "Tripod"), 
            new Category(8, 4, "Computers"),
            new Category(9, 8, "Laptops"),
            new Category(10, 0, "Empty"),
            new Category(11, 12, "Invalid")
        };

        var result = someData.ToTree(x => x.Id, x => x.ParentId);

        PrintTree(result);

        result.Length.ShouldBe(3);
            
        result[0].Value.Name.ShouldBe("Sport");
        result[0].Children.Length.ShouldBe(2);
        result[0].Children[0].Value.Name.ShouldBe("Balls");
        result[0].Children[0].Children.ShouldBeEmpty();
        result[0].Children[1].Value.Name.ShouldBe("Shoes");
        result[0].Children[1].Children.ShouldBeEmpty();

        result[1].Value.Name.ShouldBe("Electronics");
        result[1].Children.Length.ShouldBe(2);
        result[1].Children[0].Value.Name.ShouldBe("Cameras");
        result[1].Children[0].Children.Length.ShouldBe(2);
        result[1].Children[0].Children[0].Value.Name.ShouldBe("Lenses");
        result[1].Children[0].Children[0].Children.ShouldBeEmpty();
        result[1].Children[0].Children[1].Value.Name.ShouldBe("Tripod");
        result[1].Children[0].Children[1].Children.ShouldBeEmpty();
        result[1].Children[1].Value.Name.ShouldBe("Computers");
        result[1].Children[1].Children.Length.ShouldBe(1);
        result[1].Children[1].Children[0].Value.Name.ShouldBe("Laptops");
        result[1].Children[1].Children[0].Children.ShouldBeEmpty();

        result[2].Value.Name.ShouldBe("Empty");
        result[2].Children.ShouldBeEmpty();
            
        void PrintTree(IEnumerable<TreeNode<Category>> tree, int indent = 0)
        {
            foreach (var c in tree)
            {
                Console.WriteLine(new string('\t', indent) + c.Value.Name);
                PrintTree(c.Children, indent + 1);
            }
        }
    }

    private sealed class Category
    {
        public int Id { get; }
        public int ParentId { get; }
        public string Name { get; }

        public Category(int id, int parentId, string name)
        {
            Id = id;
            ParentId = parentId;
            Name = name;
        }

        public override string ToString() => $"Id: {Id} - ParentId: {ParentId} - Name: {Name}";
    }
}